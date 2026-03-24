using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Module3.P2_1.Interfaces;

public interface ITransportMapper
{
    Transport FindById(int transportId);
    List<Transport> FindByMode(TransportMode mode);
    List<Transport> FindAvailable();
    List<Transport> FindAvailableByMode(TransportMode mode);
    void Insert(Transport transport);
    void Update(Transport transport);
    void Delete(int transportId);
}
