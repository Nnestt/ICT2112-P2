using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public class TruckMapper : TransportMapper
{
    public TruckMapper(AppDbContext context) : base(context)
    {
    }

    public new Truck FindById(int transportId)
    {
        return LoadSubtypeRow(transportId);
    }

    public Truck FindByTruckId(int truckId)
    {
        return Context.Trucks.First(t => t.ReadTruckId() == truckId);
    }

    public Truck FindByLicensePlate(string licensePlate)
    {
        return Context.Trucks.First(t => t.ReadLicensePlate() == licensePlate);
    }

    protected void InsertSubtypeRow(Truck truck, int transportId)
    {
        if (truck.ReadTransportId() != transportId)
        {
            throw new InvalidOperationException("Truck transport ID must match the base transport row.");
        }

        Context.Trucks.Add(truck);
        Context.SaveChanges();
    }

    protected void UpdateSubtypeRow(Truck truck)
    {
        Context.Trucks.Update(truck);
        Context.SaveChanges();
    }

    protected Truck LoadSubtypeRow(int transportId)
    {
        return Context.Trucks.First(t => t.ReadTransportId() == transportId);
    }

    protected void DeleteSubtypeRow(int transportId)
    {
        var truck = LoadSubtypeRow(transportId);
        Context.Trucks.Remove(truck);
        Context.SaveChanges();
    }
}
