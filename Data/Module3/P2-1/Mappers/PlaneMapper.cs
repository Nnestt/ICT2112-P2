using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public class PlaneMapper : TransportMapper
{
    public PlaneMapper(AppDbContext context) : base(context)
    {
    }

    public new Plane FindById(int transportId)
    {
        return LoadSubtypeRow(transportId);
    }

    public Plane FindByPlaneId(int planeId)
    {
        return Context.Planes.First(p => p.ReadPlaneId() == planeId);
    }

    public Plane FindByPlaneCallsign(string planeCallsign)
    {
        return Context.Planes.First(p => p.ReadPlaneCallsign() == planeCallsign);
    }

    protected void InsertSubtypeRow(Plane plane, int transportId)
    {
        if (plane.ReadTransportId() != transportId)
        {
            throw new InvalidOperationException("Plane transport ID must match the base transport row.");
        }

        Context.Planes.Add(plane);
        Context.SaveChanges();
    }

    protected void UpdateSubtypeRow(Plane plane)
    {
        Context.Planes.Update(plane);
        Context.SaveChanges();
    }

    protected Plane LoadSubtypeRow(int transportId)
    {
        return Context.Planes.First(p => p.ReadTransportId() == transportId);
    }

    protected void DeleteSubtypeRow(int transportId)
    {
        var plane = LoadSubtypeRow(transportId);
        Context.Planes.Remove(plane);
        Context.SaveChanges();
    }
}
