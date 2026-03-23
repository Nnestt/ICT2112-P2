using ProRental.Data.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Domain.Control;

/// <summary>
/// Factory class for loading the correct hub subtype from the database.
/// Used by TransportationHubManager to retrieve hub entities with their subtype data.
/// </summary>
public class TransportationHubFactory
{
    private readonly ITransportationHubMapper _hubMapper;

    public TransportationHubFactory(ITransportationHubMapper hubMapper)
    {
        _hubMapper = hubMapper;
    }

    /// <summary>
    /// Loads a TransportationHub by ID, including its subtype (Warehouse, Airport, or ShippingPort).
    /// </summary>
    public TransportationHub? CreateHub(int hubId)
    {
        return _hubMapper.FindById(hubId);
    }

    public Warehouse? CreateWarehouse(int hubId)
    {
        var hub = _hubMapper.FindById(hubId);
        return hub?.Warehouse;
    }

    public Airport? CreateAirport(int hubId)
    {
        var hub = _hubMapper.FindById(hubId);
        return hub?.Airport;
    }

    public ShippingPort? CreateShippingPort(int hubId)
    {
        var hub = _hubMapper.FindById(hubId);
        return hub?.ShippingPort;
    }

    /// <summary>
    /// Loads all hubs of a specific type.
    /// </summary>
    public List<TransportationHub> CreateHubsByType(HubType hubType)
    {
        return _hubMapper.FindByType(hubType);
    }
}
