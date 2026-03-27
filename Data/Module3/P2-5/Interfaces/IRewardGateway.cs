using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Data;

public interface IRewardGateway
{
    void Save(Customerreward reward);
    Customerreward? FindByOrderCarbonDataId(int orderCarbonDataId);
    List<Customerreward> FindAll();
}