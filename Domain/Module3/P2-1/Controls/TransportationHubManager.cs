using ProRental.Data.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces;

namespace ProRental.Domain.Control;

/// <summary>
/// Main control class for Feature 4 — Transportation Hub & Warehouse Carbon Tracking.
/// Implements both IHubCarbonService and IHubInfoService.
/// Contains business logic for carbon calculations.
/// </summary>
public class TransportationHubManager : IHubCarbonService, IHubInfoService
{
    private readonly TransportationHubFactory _hubFactory;
    private readonly ITransportationHubMapper _hubMapper;
    private readonly IInventoryService _inventoryService;

    public TransportationHubManager(
        TransportationHubFactory hubFactory,
        ITransportationHubMapper hubMapper,
        IInventoryService inventoryService)
    {
        _hubFactory = hubFactory;
        _hubMapper = hubMapper;
        _inventoryService = inventoryService;
    }

    // =============================================
    // IHubCarbonService implementation
    // =============================================

    /// <summary>
    /// Calculates total hub carbon emissions for the given number of hours.
    /// Only applicable to Warehouse hubs (others return 0).
    /// </summary>
    public double CalculateHubCarbon(int hubId, double hours)
    {
        var warehouse = _hubFactory.CreateWarehouse(hubId);
        if (warehouse == null) return 0;

        double emissionsPerHour = GetEmissionsPerHour(warehouse);
        return emissionsPerHour * hours;
    }

    /// <summary>
    /// Calculates CO2 emissions attributable to a specific product in a warehouse.
    /// Formula: EmissionsPerHour × (ProductWeight × Qty / TotalWarehouseVolume) × HoursStored
    /// Uses product weight as a proxy for volume since volume data is unavailable.
    /// </summary>
    public double CalculateProductStorageCarbon(int productId, int hubId)
    {
        var warehouse = _hubFactory.CreateWarehouse(hubId);
        if (warehouse == null) return 0;

        decimal productWeight = _inventoryService.GetProductWeight(productId);
        if (productWeight <= 0) return 0;

        // Use real inventory data: createdat as "stored since", totalquantity as qty
        var storageInfo = _inventoryService.GetProductStorageInfo(productId);
        double hoursStored = storageInfo?.HoursStored ?? 24;
        int quantity = storageInfo?.Quantity ?? 1;

        return CalculateProductStorageCarbonInternal(warehouse, (double)productWeight, quantity, hoursStored);
    }

    /// <summary>
    /// Gets carbon breakdown for each individual inventory item of a specific product.
    /// Each item gets its own carbon calculation based on its own storage duration.
    /// </summary>
    public List<ItemCarbonInfo> GetProductItemCarbonBreakdown(int productId, int hubId)
    {
        var warehouse = _hubFactory.CreateWarehouse(hubId);
        if (warehouse == null) return new List<ItemCarbonInfo>();

        decimal productWeight = _inventoryService.GetProductWeight(productId);
        if (productWeight <= 0) return new List<ItemCarbonInfo>();

        var allItems = _inventoryService.GetAllProductStorageInfo();
        var productItems = allItems.Where(i => i.ProductId == productId).ToList();

        var results = new List<ItemCarbonInfo>();
        foreach (var item in productItems)
        {
            double carbon = CalculateProductStorageCarbonInternal(warehouse, (double)productWeight, 1, item.HoursStored);
            double emissionRate = item.HoursStored > 0 ? carbon / item.HoursStored : 0;

            results.Add(new ItemCarbonInfo
            {
                InventoryItemId = item.InventoryItemId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                SerialNumber = item.SerialNumber,
                CarbonEmission = carbon,
                EmissionRatePerHour = emissionRate,
                HoursStored = item.HoursStored,
                StoredSince = item.StoredSince,
                Quantity = 1
            });
        }

        return results.OrderByDescending(i => i.CarbonEmission).ToList();
    }

    /// <summary>
    /// Recommends items to clear from a warehouse to reduce carbon footprint.
    /// Returns items sorted by highest carbon impact (heaviest items stored longest).
    /// </summary>
    public List<ItemCarbonInfo> RecommendItemsToClear(int hubId)
    {
        var warehouse = _hubFactory.CreateWarehouse(hubId);
        if (warehouse == null) return new List<ItemCarbonInfo>();

        // Get all inventory items with real storage data from DB
        var allItems = _inventoryService.GetAllProductStorageInfo();

        // Filter to items stored 18 months or longer (18 × 30 × 24 = 13,140 hours)
        const double EighteenMonthsInHours = 13140;
        var longStoredItems = allItems.Where(i => i.HoursStored >= EighteenMonthsInHours).ToList();

        var results = new List<ItemCarbonInfo>();
        foreach (var item in longStoredItems)
        {
            // Get product weight from DB
            decimal weight = _inventoryService.GetProductWeight(item.ProductId);
            if (weight <= 0) continue;

            double carbon = CalculateProductStorageCarbonInternal(warehouse, (double)weight, item.Quantity, item.HoursStored);
            double emissionRate = item.HoursStored > 0 ? carbon / item.HoursStored : 0;

            results.Add(new ItemCarbonInfo
            {
                InventoryItemId = item.InventoryItemId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                SerialNumber = item.SerialNumber,
                CarbonEmission = carbon,
                EmissionRatePerHour = emissionRate,
                HoursStored = item.HoursStored,
                StoredSince = item.StoredSince,
                Quantity = item.Quantity
            });
        }

        // Sort by highest carbon impact first, limit to top 15
        return results.OrderByDescending(i => i.CarbonEmission).Take(15).ToList();
    }

    /// <summary>
    /// Gets how long products have been stored in a warehouse.
    /// Uses InventoryItem.createdat as "stored since" timestamp.
    /// </summary>
    public List<ProductTimeInfo> GetProductTimeInWarehouse(int hubId)
    {
        var warehouse = _hubFactory.CreateWarehouse(hubId);
        if (warehouse == null) return new List<ProductTimeInfo>();

        // Get all inventory items with real storage data from DB
        var allItems = _inventoryService.GetAllProductStorageInfo();

        var results = allItems.Select(item => new ProductTimeInfo
        {
            InventoryItemId = item.InventoryItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            SerialNumber = item.SerialNumber,
            HoursStored = item.HoursStored,
            StoredSince = item.StoredSince
        }).ToList();

        // Sort by longest storage time first
        return results.OrderByDescending(p => p.HoursStored).ToList();
    }

    // =============================================
    // Private calculation helpers
    // =============================================

    /// <summary>
    /// Returns the total warehouse CO2 emissions per hour by summing all emission rate components.
    /// </summary>
    private double GetEmissionsPerHour(Warehouse warehouse)
    {
        return (warehouse.GetClimateControlEmissionRate() ?? 0)
             + (warehouse.GetLightingEmissionRate() ?? 0)
             + (warehouse.GetSecuritySystemEmissionRate() ?? 0);
    }

    /// <summary>
    /// Calculates the carbon emissions attributable to a product stored in a warehouse.
    /// Formula: EmissionsPerHour × (productWeight × quantity / totalWarehouseVolume) × hoursStored
    /// </summary>
    private double CalculateProductStorageCarbonInternal(Warehouse warehouse, double productWeight, int quantity, double hoursStored)
    {
        double volume = warehouse.GetTotalWarehouseVolume() ?? 0;
        if (volume <= 0) return 0;

        double emissionsPerHour = GetEmissionsPerHour(warehouse);
        double proportion = (productWeight * quantity) / volume;

        return emissionsPerHour * proportion * hoursStored;
    }

    // =============================================
    // IHubInfoService implementation
    // =============================================

    /// <summary>
    /// Finds the nearest warehouse to the given coordinates using Euclidean distance.
    /// Returns the warehouse code of the nearest operational warehouse, or null if none found.
    /// </summary>
    public string? FindNearestWarehouse(double latitude, double longitude)
    {
        var warehouseHubs = _hubMapper.FindByType(HubType.WAREHOUSE);

        TransportationHub? nearest = null;
        double minDistance = double.MaxValue;

        foreach (var hub in warehouseHubs)
        {
            if (!hub.IsOperational()) continue;

            double distance = Math.Sqrt(
                Math.Pow(hub.GetLatitude() - latitude, 2) +
                Math.Pow(hub.GetLongitude() - longitude, 2));

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = hub;
            }
        }

        return nearest?.Warehouse?.GetWarehouseCode();
    }

    /// <summary>
    /// Gets full hub information by hub ID, including subtype data.
    /// </summary>
    public TransportationHub? GetHubInfo(int hubId)
    {
        return _hubFactory.CreateHub(hubId);
    }

    /// <summary>
    /// Gets all transportation hubs.
    /// </summary>
    public List<TransportationHub> GetAllHubs()
    {
        return _hubMapper.FindAll();
    }

    /// <summary>
    /// Gets all products as lightweight dropdown items for UI selection.
    /// Delegates to InventoryService so the controller doesn't need to depend on it directly.
    /// </summary>
    public List<ProductDropdownItem> GetAllProductDropdownItems()
    {
        return _inventoryService.GetAllProductDropdownItems();
    }

    /// <summary>
    /// Gets storage info for all product items in the warehouse.
    /// Delegates to InventoryService so the controller doesn't need to depend on it directly.
    /// </summary>
    public List<ProductStorageInfo> GetAllProductStorageInfo()
    {
        return _inventoryService.GetAllProductStorageInfo();
    }
}
