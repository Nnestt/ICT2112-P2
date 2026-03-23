using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

/// <summary>
/// DUMMY interface — temporary stand-in for Module 2's inventory service.
/// Replace with the real IInventoryService from Module 2 when available.
/// </summary>
public interface IInventoryService
{
    Product? GetProductById(int productId);
    decimal GetProductWeight(int productId);
    List<ProductDropdownItem> GetAllProductDropdownItems();
    ProductStorageInfo? GetProductStorageInfo(int productId);
    List<ProductStorageInfo> GetAllProductStorageInfo();
}

/// <summary>
/// Lightweight DTO for product dropdowns (avoids needing public getters on Product entity).
/// </summary>
public class ProductDropdownItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO containing storage duration info for an individual inventory item.
/// </summary>
public class ProductStorageInfo
{
    public int InventoryItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime StoredSince { get; set; }
    public double HoursStored { get; set; }
}
