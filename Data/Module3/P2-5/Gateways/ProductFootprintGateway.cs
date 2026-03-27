using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Module3.P2_5.Entities;
using System.Reflection;

namespace ProRental.Data.Module3.P2_5.Gateways;

public sealed class ProductFootprintGateway : IProductFootprintGateway
{
    private readonly AppDbContext _dbContext;

    public ProductFootprintGateway(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<ChartData> GetHourlyChartData()
    {
        return _dbContext.Productfootprints
            .AsEnumerable()
            .GroupBy(GetCalculatedAt)
            .Select(group => new ChartData(
                group.Key.ToString("yyyy-MM-dd HH:mm"),
                Math.Round(group.Sum(GetTotalCo2), 2)))
            .OrderBy(chart => chart.Label)
            .ToList();
    }

    public List<ChartData> GetProductGraphData()
    {
        var namesById = _dbContext.Productdetails
            .Select(detail => new
            {
                Id = EF.Property<int>(detail, "Productid"),
                Name = EF.Property<string>(detail, "Name")
            })
            .ToDictionary(entry => entry.Id, entry => entry.Name);

        return _dbContext.Productfootprints
            .AsEnumerable()
            .GroupBy(GetProductId)
            .Select(group =>
            {
                var productId = group.Key;
                var name = namesById.TryGetValue(productId, out var productName)
                    ? productName
                    : $"Product {productId}";

                return new ChartData(
                    name,
                    Math.Round(group.Sum(GetTotalCo2), 2));
            })
            .OrderByDescending(graph => graph.Value)
            .ThenBy(graph => graph.Label)
            .ToList();
    }

    public List<ChartData> GetHotspotData(int top = 5)
    {
        var namesById = _dbContext.Productdetails
            .Select(detail => new
            {
                Id = EF.Property<int>(detail, "Productid"),
                Name = EF.Property<string>(detail, "Name")
            })
            .ToDictionary(entry => entry.Id, entry => entry.Name);

        return _dbContext.Productfootprints
            .AsEnumerable()
            .GroupBy(GetProductId)
            .Select(group =>
            {
                var productId = group.Key;
                var name = namesById.TryGetValue(productId, out var productName)
                    ? productName
                    : $"Product {productId}";

                return new ChartData(
                    name,
                    Math.Round(group.Average(GetTotalCo2), 2));
            })
            .OrderByDescending(hotspot => hotspot.Value)
            .ThenBy(hotspot => hotspot.Label)
            .Take(top)
            .ToList();
    }

    public List<ProductFootprintListItem> GetAllFootprints()
    {
        return _dbContext.Productfootprints
            .Join(
                _dbContext.Products,
                footprint => EF.Property<int>(footprint, "Productid"),
                product => EF.Property<int>(product, "Productid"),
                (footprint, product) => new
                {
                    Footprint = footprint,
                    Product = product
                })
            .Join(
                _dbContext.Ecobadges,
                item => EF.Property<int>(item.Footprint, "Badgeid"),
                badge => EF.Property<int>(badge, "Badgeid"),
                (item, badge) => new ProductFootprintListItem(
                    EF.Property<int>(item.Footprint, "Productcarbonfootprintid"),
                    EF.Property<int>(item.Footprint, "Productid"),
                    EF.Property<string>(item.Product, "Sku"),
                    EF.Property<string>(badge, "Badgename"),
                    EF.Property<double?>(item.Footprint, "Producttoxicpercentage"),
                    EF.Property<double>(item.Footprint, "Totalco2"),
                    EF.Property<DateTime>(item.Footprint, "Calculatedat")))
            .AsEnumerable()
            .OrderByDescending(footprint => footprint.CalculatedAt)
            .ThenBy(footprint => footprint.ProductName)
            .ToList();
    }

    public bool DeleteFootprint(int productCarbonFootprintId)
    {
        var productFootprint = _dbContext.Productfootprints
            .FirstOrDefault(footprint => EF.Property<int>(footprint, "Productcarbonfootprintid") == productCarbonFootprintId);

        if (productFootprint is null)
        {
            return false;
        }

        _dbContext.Productfootprints.Remove(productFootprint);
        _dbContext.SaveChanges();
        return true;
    }

    public ProductFootprintCalculationResult SaveCalculatedFootprint(int productId, double toxicPercentage, double totalCo2)
    {
        // Match the calculated footprint to the first EcoBadge threshold that can contain it.
        // If the footprint exceeds all configured thresholds, use the highest-threshold badge.
        var badgeId = _dbContext.Ecobadges
            .Where(badge => EF.Property<double>(badge, "Maxcarbong") >= totalCo2)
            .OrderBy(badge => EF.Property<double>(badge, "Maxcarbong"))
            .Select(badge => EF.Property<int>(badge, "Badgeid"))
            .FirstOrDefault();

        if (badgeId == 0)
        {
            badgeId = _dbContext.Ecobadges
                .OrderByDescending(badge => EF.Property<double>(badge, "Maxcarbong"))
                .Select(badge => EF.Property<int>(badge, "Badgeid"))
                .FirstOrDefault();
        }

        if (badgeId == 0)
        {
            throw new InvalidOperationException("Unable to save product footprint because no EcoBadge records exist.");
        }

        var productFootprint = _dbContext.Productfootprints
            .FirstOrDefault(footprint => EF.Property<int>(footprint, "Productid") == productId);

        if (productFootprint is null)
        {
            productFootprint = new Productfootprint();
            _dbContext.Productfootprints.Add(productFootprint);
        }

        var entry = _dbContext.Entry(productFootprint);
        entry.Property("Productid").CurrentValue = productId;
        entry.Property("Badgeid").CurrentValue = badgeId;
        entry.Property("Producttoxicpercentage").CurrentValue = toxicPercentage;
        entry.Property("Totalco2").CurrentValue = totalCo2;
        entry.Property("Calculatedat").CurrentValue = DateTime.UtcNow;

        _dbContext.SaveChanges();

        var calculatedAt = entry.Property<DateTime>("Calculatedat").CurrentValue;
        return new ProductFootprintCalculationResult(totalCo2, calculatedAt);
    }

    private static DateTime GetCalculatedAt(Productfootprint footprint)
    {
        return ReadMember<DateTime>(footprint, "Calculatedat", "_calculatedat");
    }

    private static int GetProductId(Productfootprint footprint)
    {
        return ReadMember<int>(footprint, "Productid", "_productid");
    }

    private static double GetTotalCo2(Productfootprint footprint)
    {
        return ReadMember<double>(footprint, "Totalco2", "_totalco2");
    }

    private static T ReadMember<T>(object source, string propertyName, string fieldName)
    {
        var type = source.GetType();

        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property?.GetValue(source) is T propertyValue)
        {
            return propertyValue;
        }

        var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field?.GetValue(source) is T fieldValue)
        {
            return fieldValue;
        }

        throw new InvalidOperationException($"Unable to read '{propertyName}' from {type.Name}.");
    }
}
