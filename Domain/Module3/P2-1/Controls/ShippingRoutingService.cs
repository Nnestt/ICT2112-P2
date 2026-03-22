using ProRental.Data.UnitOfWork;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class ShippingRoutingService : IRoutingService
{
    private readonly AppDbContext _context;

    public ShippingRoutingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.DeliveryRoute> CreateRouteAsync(
        RoutingRequest request,
        CancellationToken cancellationToken = default)
    {
        var route = new Domain.Entities.DeliveryRoute();
        route.SetOriginAddress("ProRental Warehouse");
        route.SetDestinationAddress(request.DestinationAddress);
        route.SetTotalDistanceKm(CalculateDistanceKm(request.PreferenceType, request.WeightKg, request.Quantity));
        route.SetIsValid(true);

        await _context.DeliveryRoutes.AddAsync(route, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return route;
    }

    private static double CalculateDistanceKm(PreferenceType preferenceType, double weightKg, int quantity)
    {
        var baseDistance = preferenceType switch
        {
            PreferenceType.FAST => 18d,
            PreferenceType.CHEAP => 42d,
            _ => 26d
        };

        var loadOffset = Math.Max(weightKg - 1d, 0d) + Math.Max(quantity - 1, 0);
        return Math.Round(baseDistance + loadOffset, 2, MidpointRounding.AwayFromZero);
    }
}
