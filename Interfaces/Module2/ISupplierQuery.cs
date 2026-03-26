namespace ProRental.Interfaces;

/// <summary>
/// Stub interface for supplier lookups.
/// AnalyticsControl uses this to resolve supplier names for trend analytics.
/// Implemented by the Trusted Supplier Registry module.
/// </summary>
public interface ISupplierQuery
{
    Task<SupplierDto?> GetSupplierByIdAsync(int supplierID);
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
}

public class SupplierDto
{
    public int SupplierID { get; set; }
    public string Name { get; set; } = string.Empty;
}
