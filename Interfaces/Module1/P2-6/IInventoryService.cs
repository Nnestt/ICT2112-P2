namespace ProRental.Interfaces.Domain;

public interface IInventoryService
{
    bool ProcessLoan(int orderId, int customerId, DateTime startDate, DateTime dueDate,
                     Dictionary<int, int> productQuantities);
}