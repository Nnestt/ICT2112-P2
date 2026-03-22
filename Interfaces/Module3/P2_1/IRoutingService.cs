using ProRental.Domain.Entities;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IRoutingService
{
    Task<DeliveryRoute> CreateRouteAsync(
        RoutingRequest request,
        CancellationToken cancellationToken = default);
}
