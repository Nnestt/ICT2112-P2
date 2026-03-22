namespace ProRental.Interfaces.Module3.P2_1;

public interface ITransportCarbonService
{
    double CalculateLegCarbon(int quantity, double weightKg, double distanceKm, double storageCo2);
    double CalculateRouteCarbon(IReadOnlyList<double> legCarbonValues);
    double CalculateCarbonSurcharge(double totalCarbonFootprint, double surchargeRate);
}
