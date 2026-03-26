using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Inventoryitem
{
    private InventoryStatus? _status;
    private InventoryStatus? Status { get => _status; set => _status = value; }

    // Factory Methods
    public static Inventoryitem Create(int productId, string serialNumber, InventoryStatus status, DateTime? expiryDate)
    {
        var item = new Inventoryitem();
        item.SetProductId(productId);
        item.SetSerialNumber(serialNumber);
        item.SetStatus(status);
        item.SetCreatedDate(DateTime.UtcNow);
        item.SetUpdatedDate(DateTime.UtcNow);
        item.SetExpiryDate(expiryDate);
        return item;
    }

    public static Inventoryitem CreateReserved(int productId)
    {
        var item = new Inventoryitem();
        item.SetProductId(productId);
        item.SetSerialNumber($"TEMP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}");
        item.SetStatus(InventoryStatus.RESERVED);
        item.SetCreatedDate(DateTime.UtcNow);
        item.SetUpdatedDate(DateTime.UtcNow);
        item.SetExpiryDate(null);
        return item;
    }

    // Business Methods
    public void UpdateStatusAndDate(InventoryStatus status)
    {
        _status = status;
        _updatedat = DateTime.UtcNow;
    }

    public void Update(int productId, string serialNumber, InventoryStatus status, DateTime? expiryDate)
    {
        SetProductId(productId);
        SetSerialNumber(serialNumber);
        SetStatus(status);
        SetExpiryDate(expiryDate);
        SetUpdatedDate(DateTime.UtcNow);
    }

    // Getters
    public int GetInventoryItemId() => _inventoryid;
    public int GetProductId() => _productid;
    public string GetSerialNumber() => _serialnumber;
    public InventoryStatus? GetStatus() => _status;
    public DateTime GetCreatedDate() => _createdat;
    public DateTime GetUpdatedDate() => _updatedat;
    public DateTime? GetExpiryDate() => _expirydate;

    // Setters (Private)
    private void SetInventoryItemId(int id) => _inventoryid = id;
    private void SetProductId(int productId) => _productid = productId;
    private void SetSerialNumber(string serialNumber) => _serialnumber = serialNumber;
    private void SetStatus(InventoryStatus status) => _status = status;
    private void SetCreatedDate(DateTime createdDate) => _createdat = createdDate;
    private void SetUpdatedDate(DateTime updatedDate) => _updatedat = updatedDate;
    private void SetExpiryDate(DateTime? expiryDate) => _expirydate = expiryDate;
}