using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class ShippingTransportCarbonService : ITransportCarbonService
{
    public Task<TransportQuote> QuoteAsync(
        DeliveryRoute route,
        OrderShippingContext context,
        PreferenceType preferenceType,
        CancellationToken cancellationToken = default)
    {
        if (route.GetIsValid() is false)
        {
            throw new InvalidOperationException("A shipping quote cannot be created for an invalid route.");
        }

        var distanceKm = Math.Max(route.GetTotalDistanceKm() ?? 1d, 1d);
        var loadFactor = Math.Max(context.WeightKg, 1d) * Math.Max(context.Quantity, 1);

        var quote = preferenceType switch
        {
            PreferenceType.FAST => CreateQuote(
                distanceKm,
                loadFactor,
                deliveryDays: 1,
                transportMode: TransportMode.PLANE,
                displayName: "Fastest",
                transportModeLabel: "Plane"),
            PreferenceType.CHEAP => CreateQuote(
                distanceKm,
                loadFactor,
                deliveryDays: 5,
                transportMode: TransportMode.SHIP,
                displayName: "Cheapest",
                transportModeLabel: "Ship"),
            _ => CreateQuote(
                distanceKm,
                loadFactor,
                deliveryDays: 4,
                transportMode: TransportMode.TRAIN,
                displayName: "Greenest",
                transportModeLabel: "Train")
        };

        return Task.FromResult(quote);
    }

    private static TransportQuote CreateQuote(
        double distanceKm,
        double loadFactor,
        int deliveryDays,
        TransportMode transportMode,
        string displayName,
        string transportModeLabel)
    {
        var cost = transportMode switch
        {
            TransportMode.PLANE => 18m + (decimal)distanceKm * 0.80m + (decimal)loadFactor * 1.20m,
            TransportMode.SHIP => 8m + (decimal)distanceKm * 0.25m + (decimal)loadFactor * 0.50m,
            _ => 12m + (decimal)distanceKm * 0.35m + (decimal)loadFactor * 0.45m
        };

        var carbonFootprintKg = transportMode switch
        {
            TransportMode.PLANE => distanceKm * 0.95 + loadFactor * 0.40,
            TransportMode.SHIP => distanceKm * 0.22 + loadFactor * 0.18,
            _ => distanceKm * 0.12 + loadFactor * 0.10
        };

        return new TransportQuote(
            Cost: decimal.Round(cost, 2, MidpointRounding.AwayFromZero),
            CarbonFootprintKg: Math.Round(carbonFootprintKg, 2, MidpointRounding.AwayFromZero),
            DeliveryDays: deliveryDays,
            TransportMode: transportMode,
            DisplayName: displayName,
            TransportModeLabel: transportModeLabel);
    }
}
