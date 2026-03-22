using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Domain.Module3.P2_1.Factories;

public class TransportationFactory
{
    private readonly ITransportMapper _transportMapper;

    public TransportationFactory(ITransportMapper transportMapper)
    {
        _transportMapper = transportMapper;
    }

    public Transport CreateTransport(string transportationType)
    {
        if (!Enum.TryParse<TransportMode>(transportationType, true, out var mode))
        {
            throw new ArgumentOutOfRangeException(nameof(transportationType));
        }

        return _transportMapper.FindByMode(mode).First();
    }
}
