using ProRental.Interfaces.Domain;
using ProRental.Data.UnitOfWork;

namespace ProRental.Domain.Controls;

internal sealed class ProductFootprintService : IProductFootprintService
{
    private readonly AppDbContext _db;

    public ProductFootprintService(AppDbContext db)
    {
        _db = db;
    }

    public double CalculateProductFootprint(int orderId)
    {
        var orderItems = _db.Orderitems
            .ToList()
            .Where(oi => oi.GetOrderid() == orderId)
            .ToList();

        double total = 0;

        foreach (var item in orderItems)
        {
            var footprint = _db.Productfootprints
                .ToList()
                .FirstOrDefault(p => p.GetProductid() == item.GetProductid());

            if (footprint != null)
            {
                total += footprint.GetTotalco2() * item.GetQuantity();
            }
        }

        return total;
    }
}