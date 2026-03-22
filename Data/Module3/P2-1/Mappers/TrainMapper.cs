using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public class TrainMapper : TransportMapper
{
    public TrainMapper(AppDbContext context) : base(context)
    {
    }

    public new Train FindById(int transportId)
    {
        return LoadSubtypeRow(transportId);
    }

    public Train FindByTrainId(int trainId)
    {
        return Context.Trains.First(t => t.ReadTrainId() == trainId);
    }

    public Train FindByTrainNumber(string trainNumber)
    {
        return Context.Trains.First(t => t.ReadTrainNumber() == trainNumber);
    }

    protected void InsertSubtypeRow(Train train, int transportId)
    {
        if (train.ReadTransportId() != transportId)
        {
            throw new InvalidOperationException("Train transport ID must match the base transport row.");
        }

        Context.Trains.Add(train);
        Context.SaveChanges();
    }

    protected void UpdateSubtypeRow(Train train)
    {
        Context.Trains.Update(train);
        Context.SaveChanges();
    }

    protected Train LoadSubtypeRow(int transportId)
    {
        return Context.Trains.First(t => t.ReadTransportId() == transportId);
    }

    protected void DeleteSubtypeRow(int transportId)
    {
        var train = LoadSubtypeRow(transportId);
        Context.Trains.Remove(train);
        Context.SaveChanges();
    }
}
