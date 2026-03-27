using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Data;

namespace ProRental.Data;

public class RewardGateway : IRewardGateway
{
    private readonly AppDbContext _db;

    public RewardGateway(AppDbContext db)
    {
        _db = db;
    }

    public void Save(Customerreward reward)
    {
        _db.Customerrewards.Add(reward);
        _db.SaveChanges();
    }

    public Customerreward? FindByOrderCarbonDataId(int orderCarbonDataId)
        => _db.Customerrewards.ToList()
              .FirstOrDefault(r => r.GetOrdercarbondataid() == orderCarbonDataId);

    public List<Customerreward> FindAll()
        => _db.Customerrewards
              .ToList()
              .OrderByDescending(r => r.GetCreatedat())
              .ToList();
}