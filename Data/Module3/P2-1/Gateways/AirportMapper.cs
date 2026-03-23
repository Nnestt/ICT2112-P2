using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Gateways;

/// <summary>
/// Handles Airport-specific persistence operations.
/// Inherits template methods from AbstractTransportationHubMapper.
/// </summary>
public class AirportMapper : AbstractTransportationHubMapper
{
    public AirportMapper(AppDbContext context) : base(context) { }

    public override TransportationHub? FindById(int hubId)
    {
        return _context.TransportationHubs
            .Include(h => h.Airport)
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }

    public Airport? FindByAirportCode(string airportCode)
    {
        return _context.Airports
            .Include(a => a.Hub)
            .FirstOrDefault(a => EF.Property<string>(a, "AirportCode") == airportCode);
    }

    public List<Airport> FindByAirportName(string airportName)
    {
        return _context.Airports
            .Include(a => a.Hub)
            .Where(a => EF.Property<string>(a, "AirportName") == airportName)
            .ToList();
    }

    public override List<TransportationHub> FindByType(HubType hubType)
    {
        return _context.TransportationHubs
            .Include(h => h.Airport)
            .Where(h => EF.Property<HubType?>(h, "HubType") == hubType)
            .ToList();
    }

    public override List<TransportationHub> FindAll()
    {
        return _context.TransportationHubs
            .Include(h => h.Airport)
            .Where(h => h.Airport != null)
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
        if (hub.Airport == null) return;
        _context.Airports.Add(hub.Airport);
        _context.SaveChanges();
    }

    protected override void UpdateSubtypeRow(TransportationHub hub)
    {
        if (hub.Airport == null) return;
        _context.Airports.Update(hub.Airport);
        _context.SaveChanges();
    }

    protected override void DeleteSubtypeRow(int hubId)
    {
        var airport = _context.Airports
            .FirstOrDefault(a => EF.Property<int>(a, "HubId") == hubId);
        if (airport != null)
        {
            _context.Airports.Remove(airport);
            _context.SaveChanges();
        }
    }

    protected override TransportationHub? LoadSubtypeRow(int hubId)
    {
        return _context.TransportationHubs
            .Include(h => h.Airport)
            .FirstOrDefault(h => EF.Property<int>(h, "HubId") == hubId);
    }
}
