using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Data;

namespace ProRental.Data;

public class OrderCarbonDataGateway : IOrderCarbonDataGateway
{
    private readonly AppDbContext _db;

    public OrderCarbonDataGateway(AppDbContext db)
    {
        _db = db;
    }

    public void Save(Ordercarbondatum data)
    {
        _db.Ordercarbondata.Add(data);
        _db.SaveChanges();
    }

    public Ordercarbondatum? FindByOrderId(int orderId)
        => _db.Ordercarbondata.ToList().FirstOrDefault(d => d.GetOrderid() == orderId);

    public List<Ordercarbondatum> FindAll()
        => _db.Ordercarbondata
              .ToList()
              .OrderByDescending(d => d.GetCalculatedat())
              .ToList();
}