using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Gateways;

/// <summary>
/// Handles Warehouse-specific persistence operations.
/// Inherits template methods from AbstractTransportationHubMapper.
/// </summary>
public class WarehouseMapper : AbstractTransportationHubMapper
{
    public WarehouseMapper(AppDbContext context) : base(context) { }

    public override TransportationHub? FindById(int hubId)
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }

    public Warehouse? FindByWarehouseCode(string warehouseCode)
    {
        return _context.Warehouses
            .Include(w => w.Hub)
            .FirstOrDefault(w => EF.Property<string>(w, "WarehouseCode") == warehouseCode);
    }

    public override List<TransportationHub> FindByType(HubType hubType)
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .Where(h => EF.Property<HubType?>(h, "HubType") == hubType)
            .ToList();
    }

    public override List<TransportationHub> FindAll()
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .Where(h => h.Warehouse != null)
            .ToList();
    }

    public override void Insert(TransportationHub hub)
    {
        int hubId = InsertHubRow(hub);
        InsertSubtypeRow(hub, hubId);
    }

    public override void Update(TransportationHub hub)
    {
        UpdateHubRow(hub);
        UpdateSubtypeRow(hub);
    }

    public override void Delete(int hubId)
    {
        DeleteSubtypeRow(hubId);
        DeleteHubRow(hubId);
    }

    protected override void InsertSubtypeRow(TransportationHub hub, int hubId)
    {
        if (hub.Warehouse == null) return;
        _context.Warehouses.Add(hub.Warehouse);
        _context.SaveChanges();
    }

    protected override void UpdateSubtypeRow(TransportationHub hub)
    {
        if (hub.Warehouse == null) return;
        _context.Warehouses.Update(hub.Warehouse);
        _context.SaveChanges();
    }

    protected override void DeleteSubtypeRow(int hubId)
    {
        var warehouse = _context.Warehouses
            .FirstOrDefault(w => EF.Property<int>(w, "HubId") == hubId);
        if (warehouse != null)
        {
            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();
        }
    }

    protected override TransportationHub? LoadSubtypeRow(int hubId)
    {
        return _context.TransportationHubs
            .Include(h => h.Warehouse)
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }
}
