// This interface is owned by Module 1 (P2-6).
// P2-5 depends on it to retrieve order and product details for packaging profile creation.
using ProRental.Domain.Entities;

namespace ProRental.Data.Module1.Interfaces;

public interface IOrderService
{
    List<Order> GetOrders();
}
