using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Controls;

public class RouteDistanceCalculator : IRouteDistanceCalculator
{
    private static readonly Dictionary<(string StartPoint, string EndPoint), double> DistancesByLeg = new()
    {
        { ("Warehouse A", "Port Hub"), 10d },
        { ("Port Hub", "Destination Port"), 100d },
        { ("Destination Port", "Customer"), 5d },
        { ("Airport Origin", "Airport Destination"), 30d },
        { ("Rail Hub", "City Hub"), 20d },
        { ("City Hub", "Customer"), 8d }
    };

    public double CalculateDistanceKm(string startPoint, string endPoint)
    {
        if (!DistancesByLeg.TryGetValue((startPoint, endPoint), out var distanceKm))
        {
            throw new KeyNotFoundException(
                $"No test distance configured for route leg '{startPoint}' -> '{endPoint}'.");
        }

        return distanceKm;
    }
}
