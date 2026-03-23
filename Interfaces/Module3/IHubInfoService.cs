using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

/// <summary>
/// Exposes hub information methods.
/// Used by controllers (dependency inversion — controller depends on this, not on TransportationHubManager directly).
/// </summary>
public interface IHubInfoService
{
    string? FindNearestWarehouse(double latitude, double longitude);
    TransportationHub? GetHubInfo(int hubId);
    List<TransportationHub> GetAllHubs();

    /// <summary>
    /// Gets all products as lightweight dropdown items for UI selection.
    /// </summary>
    List<ProductDropdownItem> GetAllProductDropdownItems();
}
