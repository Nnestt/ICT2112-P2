using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Domain;

public interface IRewardsControl
{
    // ── Class diagram methods ─────────────────────────────────────────────────
    Ordercarbondatum CreateOrderCarbonData(int orderId, double totalCarbon);
    int CalculateEcoScore(int orderId);
    Customerreward? DetermineReward(int orderId);

    // ── Dashboard data retrieval ──────────────────────────────────────────────
    List<Order> GetAllOrders();
    List<Ordercarbondatum> GetAllCarbonRecords();
    List<Customerreward> GetAllRewards();
}