using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Interfaces.Module3.P2_1;

/// <summary>
/// Feature 3 routing contract consumed by Feature 1. The prototype implementation is
/// currently a local adapter, but Feature 1 still depends on a DeliveryRoute result.
/// by: ernest
/// </summary>
public interface IRoutingService
{
    DeliveryRoute CreateMultiModalRoute(
        string origin,
        string destination,
        List<TransportMode> modes);
}
