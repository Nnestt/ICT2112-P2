using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Feature 1 application service. It first exposes lightweight preference cards and
/// only generates a real route, quote, and persisted shipping option after selection.
/// by: ernest
/// </summary>
public sealed class ShippingOptionManager : IShippingOptionService
{
    private const string DefaultOrigin = "ProRental Warehouse";

    private static readonly PreferenceType[] PreferenceOrder =
    [
        PreferenceType.FAST,
        PreferenceType.CHEAP,
        PreferenceType.GREEN
    ];

    private readonly IShippingOptionMapper _shippingOptionMapper;
    private readonly IOrderService _orderService;
    private readonly IRoutingService _routingService;
    private readonly ITransportCarbonService _transportCarbonService;

    public ShippingOptionManager(
        IShippingOptionMapper shippingOptionMapper,
        IOrderService orderService,
        IRoutingService routingService,
        ITransportCarbonService transportCarbonService)
    {
        _shippingOptionMapper = shippingOptionMapper;
        _orderService = orderService;
        _routingService = routingService;
        _transportCarbonService = transportCarbonService;
    }

    public async Task<IReadOnlyList<ShippingPreferenceCard>> GetPreferenceChoicesForOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var context = await _orderService.GetShippingContextAsync(orderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{orderId}' was not found.");

        return PreferenceOrder
            .Select(preferenceType =>
            {
                var profile = GetPreferenceProfile(preferenceType);

                return new ShippingPreferenceCard(
                    context.OrderId,
                    preferenceType,
                    profile.DisplayName,
                    profile.Description,
                    string.Join(" + ", PreferenceTypeModes.AllowedModes[preferenceType]));
            })
            .ToArray();
    }

    public async Task<ShippingSelectionResult> ConfirmPreferenceSelectionAsync(
        SelectShippingPreferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OrderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.OrderId));
        }

        var context = await _orderService.GetShippingContextAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{request.OrderId}' was not found.");

        var order = await _shippingOptionMapper.FindOrderWithCheckoutAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{request.OrderId}' was not found.");

        var checkoutId = order.GetOrderContext().CheckoutId;
        if (checkoutId <= 0)
        {
            throw new InvalidOperationException($"Order '{request.OrderId}' does not have a checkout record.");
        }

        var profile = GetPreferenceProfile(request.PreferenceType);
        var allowedModes = PreferenceTypeModes.AllowedModes.TryGetValue(request.PreferenceType, out var modes)
            ? modes
            : throw new InvalidOperationException($"No routing profile was configured for preference '{request.PreferenceType}'.");

        var route = _routingService.CreateMultiModalRoute(DefaultOrigin, context.DestinationAddress, [.. allowedModes]);
        var routeId = route.GetRouteId();
        var quote = _transportCarbonService.CalculateRouteQuote(
            route,
            Math.Max(context.Quantity, 1),
            Math.Max(context.WeightKg, 1d));

        var existingOptions = await _shippingOptionMapper.FindByOrderIdAsync(request.OrderId, cancellationToken);
        var option = existingOptions.FirstOrDefault() ?? new ShippingOption();

        option.ConfigureGeneratedOption(
            request.OrderId,
            routeId > 0 ? routeId : null,
            request.PreferenceType,
            profile.DisplayName,
            quote.Cost,
            quote.CarbonFootprintKg,
            profile.DeliveryDays,
            profile.PrimaryTransportMode);

        if (existingOptions.Count > 0)
        {
            await _shippingOptionMapper.UpdateAsync(option, cancellationToken);
        }
        else
        {
            await _shippingOptionMapper.AddAsync(option, cancellationToken);
        }

        await _shippingOptionMapper.SaveChangesAsync(cancellationToken);

        var optionId = option.GetSummary().OptionId;
        await _shippingOptionMapper.SetCheckoutSelectedOptionAsync(checkoutId, optionId, cancellationToken);
        await _shippingOptionMapper.SaveChangesAsync(cancellationToken);

        return option.GetSelectionResult() with { OrderId = request.OrderId };
    }

    private static PreferenceProfile GetPreferenceProfile(PreferenceType preferenceType)
    {
        return preferenceType switch
        {
            PreferenceType.FAST => new PreferenceProfile(
                "Fastest",
                "Prioritizes the quickest route before any pricing or carbon work is performed.",
                1,
                TransportMode.PLANE),
            PreferenceType.CHEAP => new PreferenceProfile(
                "Cheapest",
                "Defers route generation until you confirm the most cost-conscious preference.",
                5,
                TransportMode.SHIP),
            _ => new PreferenceProfile(
                "Greenest",
                "Uses the low-emission preference profile and only calculates the final route after selection.",
                4,
                TransportMode.TRAIN)
        };
    }

    private sealed record PreferenceProfile(
        string DisplayName,
        string Description,
        int DeliveryDays,
        TransportMode PrimaryTransportMode);
}
