using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Data;

public interface IOrderGateway
{
    List<Order> FindAll();
    Order? FindById(int orderId);
}