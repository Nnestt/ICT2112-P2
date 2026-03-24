using ProRental.Data.UnitOfWork;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Mocks;

/// <summary>
/// Mock routing service used until the real Feature 3 routing implementation is available.
/// It preserves the DeliveryRoute-based contract while synthesizing a route body locally.
/// by: ernest
/// </summary>
public sealed class MockRoutingService : IRoutingService
{
    private readonly AppDbContext _context;
    private static readonly IReadOnlyDictionary<TransportMode, double> SegmentDistancesKm =
        new Dictionary<TransportMode, double>
        {
            [TransportMode.PLANE] = 18d,
            [TransportMode.SHIP] = 42d,
            [TransportMode.TRAIN] = 26d,
            [TransportMode.TRUCK] = 8d
        };

    public MockRoutingService(AppDbContext context)
    {
        _context = context;
    }

    public Domain.Entities.DeliveryRoute CreateMultiModalRoute(
        string origin,
        string destination,
        List<TransportMode> modes)
    {
        if (string.IsNullOrWhiteSpace(origin))
        {
            throw new ArgumentException("Origin is required.", nameof(origin));
        }

        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("Destination is required.", nameof(destination));
        }

        if (modes is null || modes.Count == 0)
        {
            throw new ArgumentException("At least one transport mode is required.", nameof(modes));
        }

        var route = new Domain.Entities.DeliveryRoute();
        // This remains a stub route, but it now reflects the selected preference's allowed
        // mode list so Feature 1 can defer route generation until after customer selection.
        route.SetOriginAddress(origin);
        route.SetDestinationAddress(destination);
        route.SetIsValid(true);

        double totalDistanceKm = 0d;

        for (var index = 0; index < modes.Count; index++)
        {
            var mode = modes[index];
            var leg = new Domain.Entities.RouteLeg();
            var startPoint = index == 0 ? origin : $"{modes[index - 1]} Hub";
            var endPoint = index == modes.Count - 1 ? destination : $"{mode} Hub";
            var distanceKm = SegmentDistancesKm.TryGetValue(mode, out var configuredDistance)
                ? configuredDistance
                : 10d;

            leg.ConfigureLeg(
                index + 1,
                startPoint,
                endPoint,
                distanceKm,
                mode,
                index == 0,
                index == modes.Count - 1);

            route.RouteLegs.Add(leg);
            totalDistanceKm += distanceKm;
        }

        route.SetTotalDistanceKm(Math.Round(totalDistanceKm, 2, MidpointRounding.AwayFromZero));

        _context.DeliveryRoutes.Add(route);
        _context.SaveChanges();

        return route;
    }
}
