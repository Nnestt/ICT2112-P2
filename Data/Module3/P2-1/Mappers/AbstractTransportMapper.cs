using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public abstract class AbstractTransportMapper
{
    protected readonly AppDbContext Context;

    protected AbstractTransportMapper(AppDbContext context)
    {
        Context = context;
    }

    protected virtual int InsertTransportRow(Transport transport)
    {
        Context.Transports.Add(transport);
        Context.SaveChanges();
        return transport.ReadTransportId();
    }

    protected virtual void UpdateTransportRow(Transport transport)
    {
        Context.Transports.Update(transport);
        Context.SaveChanges();
    }

    protected virtual void DeleteTransportRow(int transportId)
    {
        var transport = LoadTransportRow(transportId);
        Context.Transports.Remove(transport);
        Context.SaveChanges();
    }

    protected virtual Transport LoadTransportRow(int transportId)
    {
        return Context.Transports.First(t => t.ReadTransportId() == transportId);
    }
}
