using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Controls;

/// <summary>
/// Shared Feature 2 transport-carbon implementation kept on the original calculation
/// contract so other features can compose their own workflows around it.
/// by: bryan
/// </summary>
public sealed class TransportCarbonManager : ITransportCarbonService
{
    private readonly IPricingRuleGateway _pricingRuleGateway;

    public TransportCarbonManager(IPricingRuleGateway pricingRuleGateway)
    {
        _pricingRuleGateway = pricingRuleGateway;
    }

    public double CalculateLegCarbon(int quantity, double weightKg, double distanceKm, double storageCo2)
    {
        return (quantity * weightKg * distanceKm) + storageCo2;
    }

    public double CalculateRouteCarbon(IReadOnlyList<double> legCarbonValues)
    {
        return legCarbonValues.Sum();
    }

    public double CalculateLegCarbonSurcharge(int quantity, double weightKg, double distanceKm, double storageCo2, TransportMode transportMode)
    {
        var legCarbon = CalculateLegCarbon(
            quantity,
            weightKg,
            distanceKm,
            storageCo2);

        var surchargeRate = (double)(_pricingRuleGateway.FindByTransportMode(transportMode)
            .FirstOrDefault(rule => rule.ReadIsActive())
            ?.ReadCarbonSurcharge() ?? 0m);

        return legCarbon * surchargeRate;
    }

    public double CalculateTotalCarbonSurcharge(IReadOnlyList<double> legSurcharges)
    {
        return legSurcharges.Sum();
    }

    public RouteQuoteResult CalculateRouteQuote(DeliveryRoute route, int quantity, double weightKg)
    {
        ArgumentNullException.ThrowIfNull(route);

        if (route.GetIsValid() is false)
        {
            throw new InvalidOperationException("A quote cannot be calculated for an invalid route.");
        }

        var routeLegs = route.GetOrderedRouteLegs();
        var quoteLegs = routeLegs.Count > 0
            ? routeLegs.Select(routeLeg => new QuoteLeg(
                routeLeg.GetDistanceKm() ?? 0d,
                routeLeg.GetTransportMode() ?? TransportMode.TRUCK))
            : [new QuoteLeg(route.GetTotalDistanceKm() ?? 0d, TransportMode.TRUCK)];

        var legCarbonBases = new List<double>(quoteLegs.Count());
        var pricedLegCarbonValues = new List<double>(quoteLegs.Count());
        var legSurcharges = new List<double>(quoteLegs.Count());

        foreach (var leg in quoteLegs)
        {
            var legCarbonBase = CalculateLegCarbon(quantity, weightKg, leg.DistanceKm, storageCo2: 0d);
            var pricingRule = _pricingRuleGateway.FindByTransportMode(leg.TransportMode)
                .FirstOrDefault(rule => rule.ReadIsActive());
            var baseRate = (double)(pricingRule?.ReadBaseRatePerKm() ?? 0m);

            legCarbonBases.Add(legCarbonBase);
            pricedLegCarbonValues.Add(legCarbonBase * baseRate);
            legSurcharges.Add(CalculateLegCarbonSurcharge(quantity, weightKg, leg.DistanceKm, 0d, leg.TransportMode));
        }

        var totalCarbonKg = Math.Round(CalculateRouteCarbon(legCarbonBases), 2, MidpointRounding.AwayFromZero);
        var baseCost = CalculateRouteCarbon(pricedLegCarbonValues);
        var surchargeCost = CalculateTotalCarbonSurcharge(legSurcharges);
        var totalCost = decimal.Round((decimal)(baseCost + surchargeCost), 2, MidpointRounding.AwayFromZero);

        return new RouteQuoteResult(totalCost, totalCarbonKg);
    }

    private sealed record QuoteLeg(double DistanceKm, TransportMode TransportMode);
}
