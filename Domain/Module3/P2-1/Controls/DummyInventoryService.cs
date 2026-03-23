using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces;

namespace ProRental.Domain.Control;

/// <summary>
/// DUMMY implementation of IInventoryService.
/// Temporary stand-in until Module 2 provides their real implementation.
/// Reads product data directly from AppDbContext.
/// TODO: Remove this class and replace DI registration with Module 2's real service.
/// </summary>
public class DummyInventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public DummyInventoryService(AppDbContext context)
    {
        _context = context;
    }

    public Product? GetProductById(int productId)
    {
        return _context.Products
            .Include(p => p.Productdetail)
            .FirstOrDefault(p => EF.Property<int>(p, "Productid") == productId);
    }

    public List<ProductDropdownItem> GetAllProductDropdownItems()
    {
        return _context.Set<Productdetail>()
            .Select(pd => new ProductDropdownItem
            {
                ProductId = EF.Property<int>(pd, "Productid"),
                Name = EF.Property<string>(pd, "Name")
            })
            .ToList();
    }

    public decimal GetProductWeight(int productId)
    {
        var product = GetProductById(productId);
        if (product?.Productdetail == null) return 0;

        // Access weight via the Productdetail navigation property
        // Productdetail._weight is private, so we use EF to project it
        var weight = _context.Set<Productdetail>()
            .Where(pd => EF.Property<int>(pd, "Productid") == productId)
            .Select(pd => EF.Property<decimal?>(pd, "Weight"))
            .FirstOrDefault();

        return weight ?? 0;
    }

    public List<ProductStorageInfo> GetAllProductStorageInfo()
    {
        var now = DateTime.UtcNow;

        // Get each individual inventory item
        var inventoryItems = _context.Set<Inventoryitem>()
            .Select(i => new
            {
                InventoryItemId = EF.Property<int>(i, "Inventoryid"),
                ProductId = EF.Property<int>(i, "Productid"),
                SerialNumber = EF.Property<string>(i, "Serialnumber"),
                CreatedAt = EF.Property<DateTime>(i, "Createdat")
            })
            .ToList();

        // Get product names
        var productDetails = _context.Set<Productdetail>()
            .Select(pd => new
            {
                ProductId = EF.Property<int>(pd, "Productid"),
                Name = EF.Property<string>(pd, "Name")
            })
            .ToList();

        var results = new List<ProductStorageInfo>();
        foreach (var inv in inventoryItems)
        {
            var detail = productDetails.FirstOrDefault(d => d.ProductId == inv.ProductId);
            var hoursStored = (now - inv.CreatedAt).TotalHours;

            results.Add(new ProductStorageInfo
            {
                InventoryItemId = inv.InventoryItemId,
                ProductId = inv.ProductId,
                ProductName = detail?.Name ?? "Unknown",
                SerialNumber = inv.SerialNumber,
                Quantity = 1,
                StoredSince = inv.CreatedAt,
                HoursStored = hoursStored > 0 ? hoursStored : 0
            });
        }

        return results;
    }

    public ProductStorageInfo? GetProductStorageInfo(int productId)
    {
        // Get the earliest inventory item for this product
        var inventoryData = _context.Set<Inventoryitem>()
            .Where(i => EF.Property<int>(i, "Productid") == productId)
            .Select(i => new
            {
                InventoryItemId = EF.Property<int>(i, "Inventoryid"),
                SerialNumber = EF.Property<string>(i, "Serialnumber"),
                CreatedAt = EF.Property<DateTime>(i, "Createdat")
            })
            .OrderBy(i => i.CreatedAt)
            .FirstOrDefault();

        if (inventoryData == null) return null;

        // Get product name
        var productName = _context.Set<Productdetail>()
            .Where(pd => EF.Property<int>(pd, "Productid") == productId)
            .Select(pd => EF.Property<string>(pd, "Name"))
            .FirstOrDefault() ?? "Unknown";

        // Count how many inventory items exist for this product
        var quantity = _context.Set<Inventoryitem>()
            .Where(i => EF.Property<int>(i, "Productid") == productId)
            .Count();

        var now = DateTime.UtcNow;
        var hoursStored = (now - inventoryData.CreatedAt).TotalHours;

        return new ProductStorageInfo
        {
            InventoryItemId = inventoryData.InventoryItemId,
            ProductId = productId,
            ProductName = productName,
            SerialNumber = inventoryData.SerialNumber,
            Quantity = quantity,
            StoredSince = inventoryData.CreatedAt,
            HoursStored = hoursStored > 0 ? hoursStored : 0
        };
    }
}
