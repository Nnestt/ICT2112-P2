using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class Alert
{
    private int? _staffid;
    private AlertStatus _status;
    private AlertStatus Status { get => _status; set => _status = value; }

    private DateTime? _resolvedat;

    // Factory Method
    public static Alert Create(int productId, int minThreshold, AlertStatus status, string message, int currentStock)
    {
        var alert = new Alert();
        alert.SetAlertId(0); // Will be assigned by database
        alert.SetProductId(productId);
        alert.SetMinThreshold(minThreshold);
        alert.SetAlertStatus(status);
        alert.SetCreatedAt(DateTime.UtcNow);
        alert.SetCurrentStock(currentStock);
        alert.SetMessage(message);
        return alert;
    }

    // Business Methods
    public void UpdateStatus(AlertStatus newStatus)
    {
        _status = newStatus;
        if (newStatus == AlertStatus.RESOLVED)
        {
            _resolvedat = DateTime.UtcNow;
        }
    }

    // Getters
    public int GetAlertId() => _alertid;
    public int GetProductId() => _productid;
    public AlertStatus GetAlertStatus() => _status;
    public int GetMinThreshold() => _minthreshold;
    public int GetCurrentStock() => _currentstock;
    public string GetMessage() => _message;
    public int? GetStaffId() => _staffid;
    public DateTime GetCreatedAt() => _createdat;
    public DateTime? GetResolvedAt() => _resolvedat;

    // Setters (Private)
    private void SetAlertId(int alertId) => _alertid = alertId;
    private void SetProductId(int productId) => _productid = productId;
    private void SetAlertStatus(AlertStatus status) => _status = status;
    private void SetMinThreshold(int minThreshold) => _minthreshold = minThreshold;
    private void SetCurrentStock(int currentStock) => _currentstock = currentStock;
    private void SetMessage(string message) => _message = message;
    public void SetStaffId(int? staffId) => _staffid = staffId;
    private void SetCreatedAt(DateTime createdAt) => _createdat = createdAt;
    private void SetResolvedAt(DateTime? resolvedAt) => _resolvedat = resolvedAt;
}