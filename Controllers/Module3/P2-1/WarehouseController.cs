using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces;

namespace ProRental.Controllers;

/// <summary>
/// Boundary class for warehouse carbon tracking (Feature 4).
/// Depends on IHubCarbonService and IHubInfoService (dependency inversion).
/// Never contains business logic — delegates to domain layer.
/// All endpoints use the Singapore warehouse (Hub #1) as the single warehouse.
/// </summary>
public class WarehouseController : Controller
{
    private const int WarehouseHubId = 1; // Singapore warehouse
    private const string WarehouseCarbonView = "~/Views/Module3/P2-1/WarehouseCarbon.cshtml";
    private const string ProductCarbonView = "~/Views/Module3/P2-1/ProductCarbon.cshtml";
    private const string RecommendedClearingView = "~/Views/Module3/P2-1/RecommendedClearing.cshtml";
    private const string ProductTimeView = "~/Views/Module3/P2-1/ProductTime.cshtml";

    private readonly IHubCarbonService _carbonService;
    private readonly IHubInfoService _hubInfoService;

    public WarehouseController(IHubCarbonService carbonService, IHubInfoService hubInfoService)
    {
        _carbonService = carbonService;
        _hubInfoService = hubInfoService;
    }

    /// <summary>
    /// GET: /Warehouse/GetWarehouseCarbon?hours=24
    /// </summary>
    [HttpGet]
    public IActionResult GetWarehouseCarbon(double hours = 24)
    {
        var hub = _hubInfoService.GetHubInfo(WarehouseHubId);
        if (hub == null) return NotFound("Hub not found.");

        double carbon = _carbonService.CalculateHubCarbon(WarehouseHubId, hours);
        var allStorageInfo = _carbonService.GetAllProductStorageInfo();

        double emissionsPerHour = hours > 0 ? carbon / hours : 0;

        ViewData["HubId"] = WarehouseHubId;
        ViewData["Hours"] = hours;
        ViewData["CarbonEmission"] = carbon;
        ViewData["EmissionsPerHour"] = emissionsPerHour;
        ViewData["Hub"] = hub;
        ViewData["StoredProducts"] = allStorageInfo;

        return View(WarehouseCarbonView);
    }

    /// <summary>
    /// GET: /Warehouse/GetProductCarbonEmission?productId=1
    /// </summary>
    [HttpGet]
    public IActionResult GetProductCarbonEmission(int productId = 1)
    {
        var hub = _hubInfoService.GetHubInfo(WarehouseHubId);
        var allProducts = _hubInfoService.GetAllProductDropdownItems();
        var productName = allProducts.FirstOrDefault(p => p.ProductId == productId)?.Name ?? "Unknown";

        // Per-product total carbon
        double productCarbon = _carbonService.CalculateProductStorageCarbon(productId, WarehouseHubId);

        // Per-item breakdown for selected product
        var itemBreakdown = _carbonService.GetProductItemCarbonBreakdown(productId, WarehouseHubId);

        // Total carbon across ALL products (sum of each item's carbon)
        double totalAllCarbon = 0;
        foreach (var p in allProducts)
        {
            totalAllCarbon += _carbonService.CalculateProductStorageCarbon(p.ProductId, WarehouseHubId);
        }

        // Count total inventory items for selected product
        int itemCount = itemBreakdown.Count;

        ViewData["ProductId"] = productId;
        ViewData["ProductName"] = productName;
        ViewData["HubId"] = WarehouseHubId;
        ViewData["Hub"] = hub;
        ViewData["ProductCarbon"] = productCarbon;
        ViewData["TotalAllCarbon"] = totalAllCarbon;
        ViewData["ItemBreakdown"] = itemBreakdown;
        ViewData["ItemCount"] = itemCount;
        ViewData["AllProducts"] = allProducts;

        return View(ProductCarbonView);
    }

    /// <summary>
    /// GET: /Warehouse/GetRecommendedClearing
    /// </summary>
    [HttpGet]
    public IActionResult GetRecommendedClearing()
    {
        var items = _carbonService.RecommendItemsToClear(WarehouseHubId);
        var hub = _hubInfoService.GetHubInfo(WarehouseHubId);

        ViewData["HubId"] = WarehouseHubId;
        ViewData["Hub"] = hub;
        ViewData["Items"] = items;

        return View(RecommendedClearingView);
    }

    /// <summary>
    /// GET: /Warehouse/GetProductTimeInWarehouse
    /// </summary>
    [HttpGet]
    public IActionResult GetProductTimeInWarehouse()
    {
        var products = _carbonService.GetProductTimeInWarehouse(WarehouseHubId);
        var hub = _hubInfoService.GetHubInfo(WarehouseHubId);

        ViewData["HubId"] = WarehouseHubId;
        ViewData["Hub"] = hub;
        ViewData["Products"] = products;

        return View(ProductTimeView);
    }
}
