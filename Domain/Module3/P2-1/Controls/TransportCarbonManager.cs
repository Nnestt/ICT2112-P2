using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;

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
}
