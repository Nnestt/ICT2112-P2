using Microsoft.EntityFrameworkCore;
using ProRental.Data.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Gateways;

/// <summary>
/// Abstract base class implementing ITransportationHubMapper with template methods.
/// Provides concrete hub-row operations and abstract subtype-row operations
/// that concrete mappers (WarehouseMapper, AirportMapper, ShippingPortMapper) override.
/// </summary>
public abstract class AbstractTransportationHubMapper : ITransportationHubMapper
{
    protected readonly AppDbContext _context;

    protected AbstractTransportationHubMapper(AppDbContext context)
    {
        _context = context;
    }

    // --- ITransportationHubMapper (abstract, implemented by concrete mappers) ---
    public abstract TransportationHub? FindById(int hubId);
    public abstract List<TransportationHub> FindByType(HubType hubType);
    public abstract List<TransportationHub> FindAll();
    public abstract void Insert(TransportationHub hub);
    public abstract void Update(TransportationHub hub);
    public abstract void Delete(int hubId);

    // --- Template methods for base hub row operations ---
    protected int InsertHubRow(TransportationHub hub)
    {
        _context.TransportationHubs.Add(hub);
        _context.SaveChanges();
        return hub.GetHubId();
    }

    protected void UpdateHubRow(TransportationHub hub)
    {
        _context.TransportationHubs.Update(hub);
        _context.SaveChanges();
    }

    protected void DeleteHubRow(int hubId)
    {
        var hub = _context.TransportationHubs
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
        if (hub != null)
        {
            _context.TransportationHubs.Remove(hub);
            _context.SaveChanges();
        }
    }

    protected TransportationHub? LoadHubRow(int hubId)
    {
        return _context.TransportationHubs
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }

    // --- Abstract subtype-row operations (overridden by concrete mappers) ---
    protected abstract void InsertSubtypeRow(TransportationHub hub, int hubId);
    protected abstract void UpdateSubtypeRow(TransportationHub hub);
    protected abstract void DeleteSubtypeRow(int hubId);
    protected abstract TransportationHub? LoadSubtypeRow(int hubId);
}
