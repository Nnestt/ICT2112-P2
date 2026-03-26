namespace ProRental.Interfaces.Domain;

public interface iReturnProcess
{
    bool TriggerReturnProcess(int orderId, int customerId, DateTime requestDate, List<int> inventoryItemIds);
}