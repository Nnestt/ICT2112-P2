namespace ProRental.Interfaces;

/// <summary>
/// Exposes carbon calculation methods.
/// Used by controllers (dependency inversion — controller depends on this, not on TransportationHubManager directly).
/// </summary>
public interface IHubCarbonService
{
    /// <summary>
    /// Calculates total hub carbon emissions for the given number of hours.
    /// </summary>
    double CalculateHubCarbon(int hubId, double hours);

    /// <summary>
    /// Calculates the carbon emissions attributable to a specific product stored in a warehouse.
    /// Uses product weight as a proxy for volume.
    /// </summary>
    double CalculateProductStorageCarbon(int productId, int hubId);

    /// <summary>
    /// Gets carbon breakdown for each individual inventory item of a specific product.
    /// </summary>
    List<ItemCarbonInfo> GetProductItemCarbonBreakdown(int productId, int hubId);

    /// <summary>
    /// Recommends items to clear from a warehouse to reduce carbon footprint.
    /// Returns items sorted by highest carbon impact.
    /// </summary>
    List<ItemCarbonInfo> RecommendItemsToClear(int hubId);

    /// <summary>
    /// Gets how long products have been stored in a warehouse.
    /// </summary>
    List<ProductTimeInfo> GetProductTimeInWarehouse(int hubId);

    /// <summary>
    /// Gets storage info for all product items in the warehouse.
    /// </summary>
    List<ProductStorageInfo> GetAllProductStorageInfo();
}

/// <summary>
/// DTO for items with their carbon impact, used by RecommendItemsToClear.
/// </summary>
public class ItemCarbonInfo
{
    public int InventoryItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public double CarbonEmission { get; set; }
    public double EmissionRatePerHour { get; set; }
    public double HoursStored { get; set; }
    public DateTime StoredSince { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for product storage duration, used by GetProductTimeInWarehouse.
/// </summary>
public class ProductTimeInfo
{
    public int InventoryItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public double HoursStored { get; set; }
    public DateTime StoredSince { get; set; }
}
