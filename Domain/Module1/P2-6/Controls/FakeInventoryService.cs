namespace ProRental.Interfaces.Domain;

/// <summary>
/// Temporary stub — always returns true so CreateOrder can be tested
/// without depending on the inventory team's implementation.
/// Remove once IInventoryService has a real implementation.
/// </summary>
public class FakeInventoryService : IInventoryService
{
    public bool ProcessLoan(int orderId, int customerId, DateTime startDate, DateTime dueDate,
                            Dictionary<int, int> productQuantities)
    {
        return true;
    }
}