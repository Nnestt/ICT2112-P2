using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Data.Interfaces;

/// <summary>
/// Data access interface for TransportationHub and its subtypes.
/// Consumed by Domain/Control classes only.
/// </summary>
public interface ITransportationHubMapper
{
    TransportationHub? FindById(int hubId);
    List<TransportationHub> FindByType(HubType hubType);
    List<TransportationHub> FindAll();
    void Insert(TransportationHub hub);
    void Update(TransportationHub hub);
    void Delete(int hubId);
}
