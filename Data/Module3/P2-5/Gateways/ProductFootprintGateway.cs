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
        return _dbContext.Productfootprints
            .AsEnumerable()
            .GroupBy(GetProductId)
            .Select(group => new ChartData(
                $"Product {group.Key}",
                Math.Round(group.Sum(GetTotalCo2), 2)))
            .OrderByDescending(graph => graph.Value)
            .ThenBy(graph => graph.Label)
            .ToList();
    }

    public List<ChartData> GetHotspotData(int top = 5)
    {
        return _dbContext.Productfootprints
            .AsEnumerable()
            .GroupBy(GetProductId)
            .Select(group => new ChartData(
                $"Product {group.Key}",
                Math.Round(group.Average(GetTotalCo2), 2)))
            .OrderByDescending(hotspot => hotspot.Value)
            .ThenBy(hotspot => hotspot.Label)
            .Take(top)
            .ToList();
    }

    public ProductFootprintCalculationResult SaveCalculatedFootprint(int productId, double toxicPercentage, double totalCo2)
    {
        // productfootprint.badgeid is required by the current schema,
        // so persist with the first available badge without exposing badge logic in the feature flow.
        var badgeId = _dbContext.Ecobadges
            .Select(badge => EF.Property<int>(badge, "Badgeid"))
            .OrderBy(id => id)
            .FirstOrDefault();
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
