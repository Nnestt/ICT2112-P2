using ProRental.Domain.Enums;

namespace ProRental.Interfaces.Domain;

public interface iInventoryStatusControl
{
    bool UpdateInventoryStatus(int inventoryItemId, InventoryStatus status);
}
