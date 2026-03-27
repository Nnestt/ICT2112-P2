using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Data.Module1.Interfaces;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module1.Gateways;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }
    
    public List<Order> GetOrders()
    {
        return _db.Orders
            .Include(o => o.Orderitems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Productdetail)
            .ToList();
    }
}
