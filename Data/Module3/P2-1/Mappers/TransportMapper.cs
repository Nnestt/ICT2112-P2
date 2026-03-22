using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Enums;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_1.Mappers;

public class TransportMapper : AbstractTransportMapper, ITransportMapper
{
    public TransportMapper(AppDbContext context) : base(context)
    {
    }

    public virtual Transport FindById(int transportId)
    {
        return LoadTransportRow(transportId);
    }

    public virtual List<Transport> FindByMode(TransportMode mode)
    {
        return Context.Transports
            .Where(t => EF.Property<TransportMode>(t, "TransportMode") == mode)
            .ToList();
    }

    public virtual List<Transport> FindAvailable()
    {
        return Context.Transports
            .Where(t => EF.Property<bool?>(t, "IsAvailable") == true)
            .ToList();
    }

    public virtual List<Transport> FindAvailableByMode(TransportMode mode)
    {
        return Context.Transports
            .Where(t => EF.Property<TransportMode>(t, "TransportMode") == mode)
            .Where(t => EF.Property<bool?>(t, "IsAvailable") == true)
            .ToList();
    }

    public virtual void Insert(Transport transport)
    {
        InsertTransportRow(transport);
    }

    public virtual void Update(Transport transport)
    {
        UpdateTransportRow(transport);
    }

    public virtual void Delete(int transportId)
    {
        DeleteTransportRow(transportId);
    }
}
