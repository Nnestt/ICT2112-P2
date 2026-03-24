using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public class ShipMapper : TransportMapper
{
    public ShipMapper(AppDbContext context) : base(context)
    {
    }

    public new Ship FindById(int transportId)
    {
        return LoadSubtypeRow(transportId);
    }

    public Ship FindByShipId(int shipId)
    {
        return Context.Ships.First(s => s.ReadShipId() == shipId);
    }

    public Ship FindByVesselNumber(string vesselNumber)
    {
        return Context.Ships.First(s => s.ReadVesselNumber() == vesselNumber);
    }

    protected void InsertSubtypeRow(Ship ship, int transportId)
    {
        if (ship.ReadTransportId() != transportId)
        {
            throw new InvalidOperationException("Ship transport ID must match the base transport row.");
        }

        Context.Ships.Add(ship);
        Context.SaveChanges();
    }

    protected void UpdateSubtypeRow(Ship ship)
    {
        Context.Ships.Update(ship);
        Context.SaveChanges();
    }

    protected Ship LoadSubtypeRow(int transportId)
    {
        return Context.Ships.First(s => s.ReadTransportId() == transportId);
    }

    protected void DeleteSubtypeRow(int transportId)
    {
        var ship = LoadSubtypeRow(transportId);
        Context.Ships.Remove(ship);
        Context.SaveChanges();
    }
}
