using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Data;

namespace ProRental.Data;

public class OrderGateway : IOrderGateway
{
    private readonly AppDbContext _db;

    public OrderGateway(AppDbContext db)
    {
        _db = db;
    }

    public List<Order> FindAll()
        => _db.Orders
              .ToList()
              .OrderByDescending(o => o.GetOrderdate())
              .ToList();

    public Order? FindById(int orderId)
        => _db.Orders.ToList().FirstOrDefault(o => o.GetOrderid() == orderId);
}