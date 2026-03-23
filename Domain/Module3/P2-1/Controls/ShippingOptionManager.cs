using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Domain.Module3.P2_1.Models;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Feature 1 application service. It orchestrates option generation, assembles the
/// checkout-facing snapshot from shared routing and carbon contracts, and persists
/// the customer's choice.
/// by: ernest
/// </summary>
public sealed class ShippingOptionManager : IShippingOptionService
{
    private static readonly IReadOnlyDictionary<int, ShippingOptionCarbonInput> CarbonInputFixtures =
        new Dictionary<int, ShippingOptionCarbonInput>
        {
            [1] = new()
            {
                Quantity = 3,
                ProductId = "P100",
                HubId = "HUB-A",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "Warehouse A", EndPoint = "Port Hub" },
                    new TransportRouteLegInput { Mode = TransportMode.SHIP, StartPoint = "Port Hub", EndPoint = "Destination Port" },
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "Destination Port", EndPoint = "Customer" }
                ]
            },
            [2] = new()
            {
                Quantity = 1,
                ProductId = "P200",
                HubId = "HUB-B",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.PLANE, StartPoint = "Airport Origin", EndPoint = "Airport Destination" }
                ]
            },
            [3] = new()
            {
                Quantity = 2,
                ProductId = "P300",
                HubId = "HUB-C",
                RouteLegs =
                [
                    new TransportRouteLegInput { Mode = TransportMode.TRAIN, StartPoint = "Rail Hub", EndPoint = "City Hub" },
                    new TransportRouteLegInput { Mode = TransportMode.TRUCK, StartPoint = "City Hub", EndPoint = "Customer" }
                ]
            }
        };

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
    private readonly IPricingRuleGateway _pricingRuleGateway;

    public ShippingOptionManager(
        IShippingOptionMapper shippingOptionMapper,
        IOrderService orderService,
        IRoutingService routingService,
        ITransportCarbonService transportCarbonService,
        IPricingRuleGateway pricingRuleGateway)
    {
        _shippingOptionMapper = shippingOptionMapper;
        _orderService = orderService;
        _routingService = routingService;
        _transportCarbonService = transportCarbonService;
        _pricingRuleGateway = pricingRuleGateway;
    }

    public ShippingOptionCarbonInput GetRouteCarbonInput(int shippingOptionId)
    {
        // Temporary bridge for the merged Feature 2 test endpoint until route-leg data is
        // exposed from the persisted shipping-option workflow through a dedicated contract.
        if (!CarbonInputFixtures.TryGetValue(shippingOptionId, out var input))
        {
            throw new KeyNotFoundException($"No route carbon input configured for shipping option ID {shippingOptionId}.");
        }

        return input;
    }

    public async Task<IReadOnlyList<ShippingOptionSummary>> GetShippingOptionsForOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        // Reuse persisted options when present so checkout sees stable values across reloads.
        var existingOptions = await _shippingOptionMapper.FindByOrderIdAsync(orderId, cancellationToken);
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

        // Feature 1 owns the three customer-facing preference buckets even though route
        // construction and carbon pricing are delegated to other module contracts.
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

            var snapshot = BuildGeneratedSnapshot(route, context, preferenceType);

            var option = new ShippingOption();
            var routeId = route.GetRouteId();
            option.ConfigureGeneratedOption(
                context.OrderId,
                routeId > 0 ? routeId : null,
                preferenceType,
                snapshot.DisplayName,
                snapshot.Cost,
                snapshot.CarbonFootprintKg,
                snapshot.DeliveryDays,
                snapshot.TransportMode);

            options.Add(option);
        }

        await _shippingOptionMapper.AddRangeAsync(options, cancellationToken);
        await _shippingOptionMapper.SaveChangesAsync(cancellationToken);

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

        var selectedOption = await _shippingOptionMapper.FindByIdAsync(request.OptionId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipping option '{request.OptionId}' was not found.");

        if (!selectedOption.BelongsToOrder(request.OrderId))
        {
            throw new InvalidOperationException(
                $"Shipping option '{request.OptionId}' does not belong to order '{request.OrderId}'.");
        }

        var order = await _shippingOptionMapper.FindOrderWithCheckoutAsync(request.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Order '{request.OrderId}' was not found.");

        var checkoutId = order.GetOrderContext().CheckoutId;
        if (checkoutId <= 0)
        {
            throw new InvalidOperationException($"Order '{request.OrderId}' does not have a checkout record.");
        }

        // The stored checkout selection remains the source of truth for Module 1 integration.
        var selection = selectedOption.GetSelectionResult();

        await _shippingOptionMapper.SetCheckoutSelectedOptionAsync(checkoutId, selection.OptionId, cancellationToken);
        await _shippingOptionMapper.SaveChangesAsync(cancellationToken);

        return selection with { OrderId = request.OrderId };
    }

    private static ShippingOptionSummary ToSummary(ShippingOption option)
    {
        return option.GetSummary();
    }

    private ShippingOptionSnapshot BuildGeneratedSnapshot(
        DeliveryRoute route,
        OrderShippingContext context,
        PreferenceType preferenceType)
    {
        if (route.GetIsValid() is false)
        {
            throw new InvalidOperationException("A shipping option cannot be created for an invalid route.");
        }

        var profile = GetQuoteProfile(preferenceType);
        var pricingRule = _pricingRuleGateway.FindByTransportMode(profile.TransportMode)
            .FirstOrDefault(rule => rule.ReadIsActive());
        var distanceKm = Math.Max(route.GetTotalDistanceKm() ?? 1d, 1d);
        var quantity = Math.Max(context.Quantity, 1);
        var weightKg = Math.Max(context.WeightKg, 1d);
        var baseRate = pricingRule?.ReadBaseRatePerKm() ?? 0m;
        // Feature 1 composes a checkout snapshot from the shared Feature 2 carbon calculator
        // plus the pricing rules owned by the transport-carbon implementation.
        var legCarbonBase = _transportCarbonService.CalculateLegCarbon(quantity, weightKg, distanceKm, storageCo2: 0d);
        var routeCarbonKg = _transportCarbonService.CalculateRouteCarbon([(double)baseRate * legCarbonBase]);
        var legCarbonSurcharge = _transportCarbonService.CalculateLegCarbonSurcharge(
            quantity,
            weightKg,
            distanceKm,
            0d,
            profile.TransportMode);
        var carbonSurchargeKg = _transportCarbonService.CalculateTotalCarbonSurcharge([legCarbonSurcharge]);
        var totalCarbonKg = Math.Round(routeCarbonKg + carbonSurchargeKg, 2, MidpointRounding.AwayFromZero);

        var cost = decimal.Round(
            ((decimal)distanceKm * baseRate) + ((decimal)quantity * (decimal)weightKg),
            2,
            MidpointRounding.AwayFromZero);

        return new ShippingOptionSnapshot(
            string.IsNullOrWhiteSpace(profile.DisplayName) ? preferenceType.ToString() : profile.DisplayName,
            cost,
            totalCarbonKg,
            profile.DeliveryDays,
            profile.TransportMode);
    }

    private static (TransportMode TransportMode, int DeliveryDays, string DisplayName) GetQuoteProfile(
        PreferenceType preferenceType)
    {
        return preferenceType switch
        {
            PreferenceType.FAST => (TransportMode.PLANE, 1, "Fastest"),
            PreferenceType.CHEAP => (TransportMode.SHIP, 5, "Cheapest"),
            _ => (TransportMode.TRAIN, 4, "Greenest")
        };
    }

    private sealed record ShippingOptionSnapshot(
        string DisplayName,
        decimal Cost,
        double CarbonFootprintKg,
        int DeliveryDays,
        TransportMode TransportMode);
}
