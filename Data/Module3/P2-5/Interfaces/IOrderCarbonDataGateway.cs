using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Data;

public interface IOrderCarbonDataGateway
{
    void Save(Ordercarbondatum data);
    Ordercarbondatum? FindByOrderId(int orderId);
    List<Ordercarbondatum> FindAll();
}