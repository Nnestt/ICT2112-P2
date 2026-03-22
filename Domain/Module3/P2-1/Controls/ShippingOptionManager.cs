using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Data;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class ShippingOptionManager : IShippingOptionService
{
    private static readonly PreferenceType[] PreferenceOrder =
    [
        PreferenceType.FAST,
        PreferenceType.CHEAP,
        PreferenceType.GREEN
    ];

    private readonly IShippingOptionRepository _shippingOptionRepository;
    private readonly IOrderService _orderService;
    private readonly IRoutingService _routingService;
    private readonly ITransportCarbonService _transportCarbonService;

    public ShippingOptionManager(
        IShippingOptionRepository shippingOptionRepository,
        IOrderService orderService,
        IRoutingService routingService,
        ITransportCarbonService transportCarbonService)
    {
        _shippingOptionRepository = shippingOptionRepository;
        _orderService = orderService;
        _routingService = routingService;
        _transportCarbonService = transportCarbonService;
    }

    public async Task<IReadOnlyList<ShippingOptionSummary>> GetShippingOptionsForOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var existingOptions = await _shippingOptionRepository.FindByOrderIdAsync(orderId, cancellationToken);
        if (existingOptions.Count > 0)
        {
            return existingOptions.Select(ToSummary).ToArray();
        }

        var context = await _orderService.GetShippingContextAsync(orderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{orderId}' was not found.");

        return await BuildOptionSetAsync(context, cancellationToken);
    }

    public async Task<IReadOnlyList<ShippingOptionSummary>> BuildOptionSetAsync(
        OrderShippingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var options = new List<ShippingOption>();

        foreach (var preferenceType in PreferenceOrder)
        {
            var route = await _routingService.CreateRouteAsync(
                new RoutingRequest(
                    context.OrderId,
                    context.DestinationAddress,
                    context.WeightKg,
                    context.Quantity,
                    preferenceType),
                cancellationToken);

            var quote = await _transportCarbonService.QuoteAsync(
                route,
                context,
                preferenceType,
                cancellationToken);

            var option = new ShippingOption();
            option.SetOrderId(context.OrderId);
            option.SetDisplayName(string.IsNullOrWhiteSpace(quote.DisplayName)
                ? preferenceType.ToString()
                : quote.DisplayName);
            option.SetCost(quote.Cost);
            option.SetCarbonFootprintKg(quote.CarbonFootprintKg);
            option.SetDeliveryDays(quote.DeliveryDays);
            option.UpdatePreferenceType(preferenceType);
            option.UpdateTransportMode(quote.TransportMode);

            var routeId = route.GetRouteId();
            if (routeId > 0)
            {
                option.SetRouteId(routeId);
            }

            options.Add(option);
        }

        await _shippingOptionRepository.AddRangeAsync(options, cancellationToken);
        await _shippingOptionRepository.SaveChangesAsync(cancellationToken);

        return options.Select(ToSummary).ToArray();
    }

    public async Task<ShippingSelectionResult> ApplyCustomerSelectionAsync(
        SelectShippingOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OrderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.OrderId));
        }

        if (request.OptionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.OptionId));
        }

        var selectedOption = await _shippingOptionRepository.FindByIdAsync(request.OptionId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipping option '{request.OptionId}' was not found.");

        var optionOrderId = selectedOption.GetOrderId()
            ?? throw new InvalidOperationException($"Shipping option '{request.OptionId}' is missing its order reference.");

        if (optionOrderId != request.OrderId)
        {
            throw new InvalidOperationException(
                $"Shipping option '{request.OptionId}' does not belong to order '{request.OrderId}'.");
        }

        var order = await _shippingOptionRepository.FindOrderWithCheckoutAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{request.OrderId}' was not found.");

        var checkoutId = order.GetCheckoutId();
        if (checkoutId <= 0)
        {
            throw new InvalidOperationException($"Order '{request.OrderId}' does not have a checkout record.");
        }

        await _shippingOptionRepository.SetCheckoutSelectedOptionAsync(checkoutId, selectedOption.GetOptionId(), cancellationToken);
        await _shippingOptionRepository.SaveChangesAsync(cancellationToken);

        var preferenceType = selectedOption.GetPreferenceType()
            ?? throw new InvalidOperationException($"Shipping option '{request.OptionId}' is missing its preference type.");

        return new ShippingSelectionResult(
            request.OrderId,
            selectedOption.GetOptionId(),
            preferenceType,
            selectedOption.GetCost() ?? 0m,
            selectedOption.GetCarbonFootprintKg() ?? 0d,
            selectedOption.GetDeliveryDays() ?? 0,
            selectedOption.GetTransportMode()?.ToString() ?? string.Empty);
    }

    private static ShippingOptionSummary ToSummary(ShippingOption option)
    {
        var preferenceType = option.GetPreferenceType()
            ?? throw new InvalidOperationException($"Shipping option '{option.GetOptionId()}' is missing its preference type.");

        return new ShippingOptionSummary(
            option.GetOptionId(),
            option.GetOrderId() ?? 0,
            preferenceType,
            option.GetDisplayName() ?? string.Empty,
            option.GetCost() ?? 0m,
            option.GetCarbonFootprintKg() ?? 0d,
            option.GetDeliveryDays() ?? 0,
            option.GetRouteId(),
            option.GetTransportMode(),
            option.GetTransportMode()?.ToString() ?? string.Empty);
    }
}
