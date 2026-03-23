using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Gateways;

/// <summary>
/// Concrete mapper inheriting from AbstractTransportationHubMapper.
/// Composes WarehouseMapper, AirportMapper, and ShippingPortMapper
/// to handle persistence for all TransportationHub subtypes.
/// </summary>
public class TransportationHubMapper : AbstractTransportationHubMapper
{
    // Composition: sub-mappers for each hub subtype
    private readonly WarehouseMapper _warehouseMapper;
    private readonly AirportMapper _airportMapper;
    private readonly ShippingPortMapper _shippingPortMapper;

    public TransportationHubMapper(AppDbContext context) : base(context)
    {
        _warehouseMapper = new WarehouseMapper(context);
        _airportMapper = new AirportMapper(context);
        _shippingPortMapper = new ShippingPortMapper(context);
    }

    public override TransportationHub? FindById(int hubId)
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .Include(h => h.Airport)
            .Include(h => h.ShippingPort)
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }

    public override List<TransportationHub> FindByType(HubType hubType)
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .Include(h => h.Airport)
            .Include(h => h.ShippingPort)
            .Where(h => EF.Property<HubType?>(h, "HubType") == hubType)
            .ToList();
    }

    public override List<TransportationHub> FindAll()
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .Include(h => h.Airport)
            .Include(h => h.ShippingPort)
            .ToList();
    }

    public override void Insert(TransportationHub hub)
    {
        _context.TransportationHubs.Add(hub);
        _context.SaveChanges();
    }

    public override void Update(TransportationHub hub)
    {
        _context.TransportationHubs.Update(hub);
        _context.SaveChanges();
    }

    public override void Delete(int hubId)
    {
        var hub = FindById(hubId);
        if (hub != null)
        {
            _context.TransportationHubs.Remove(hub);
            _context.SaveChanges();
        }
    }

    protected override void InsertSubtypeRow(TransportationHub hub, int hubId)
    {
        // Delegated to composed sub-mappers as needed
    }

    protected override void UpdateSubtypeRow(TransportationHub hub)
    {
        // Delegated to composed sub-mappers as needed
    }

    protected override void DeleteSubtypeRow(int hubId)
    {
        // Delegated to composed sub-mappers as needed
    }

    protected override TransportationHub? LoadSubtypeRow(int hubId)
    {
        // Delegated to composed sub-mappers as needed
        return null;
    }
}
