using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Module3.P2_5.Entities;
using System.Reflection;

namespace ProRental.Data.Module3.P2_5.Gateways;

public sealed class BuildingFootprintGateway : IBuildingFootprintGateway
{
    private readonly AppDbContext _dbContext;

    public BuildingFootprintGateway(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<ChartData> GetHourlyChartData()
    {
        return _dbContext.Buildingfootprints
            .AsEnumerable()
            .GroupBy(GetTimeHourly)
            .Select(group => new ChartData(
                group.Key.ToString("yyyy-MM-dd HH:mm"),
                Math.Round(group.Sum(GetTotalRoomCo2), 2)))
            .OrderBy(chart => chart.Label)
            .ToList();
    }

    public List<ChartData> GetZoneGraphData()
    {
        return _dbContext.Buildingfootprints
            .AsEnumerable()
            .GroupBy(footprint => GetGroupValue(footprint, "Zone"))
            .Select(group => new ChartData(
                group.Key,
                Math.Round(group.Sum(GetTotalRoomCo2), 2)))
            .OrderByDescending(graph => graph.Value)
            .ThenBy(graph => graph.Label)
            .ToList();
    }

    public List<ChartData> GetHotspotData(string groupBy, int top = 5)
    {
        var propertyName = NormalizeGroupBy(groupBy);

        return _dbContext.Buildingfootprints
            .AsEnumerable()
            .GroupBy(footprint => GetGroupValue(footprint, propertyName))
            .Select(group => new ChartData(
                group.Key,
                Math.Round(group.Average(GetTotalRoomCo2), 2)))
            .OrderByDescending(hotspot => hotspot.Value)
            .ThenBy(hotspot => hotspot.Label)
            .Take(top)
            .ToList();
    }

    private static string NormalizeGroupBy(string groupBy)
    {
        return groupBy.Trim().ToLowerInvariant() switch
        {
            "zone" => "Zone",
            "block" => "Block",
            "floor" => "Floor",
            "room" => "Room",
            _ => throw new ArgumentException(
                "groupBy must be one of: zone, block, floor, room.",
                nameof(groupBy))
        };
    }

    private static DateTime GetTimeHourly(Buildingfootprint footprint)
    {
        return ReadMember<DateTime>(footprint, "Timehourly", "_timehourly");
    }

    private static double GetTotalRoomCo2(Buildingfootprint footprint)
    {
        return ReadMember<double>(footprint, "Totalroomco2", "_totalroomco2");
    }

    private static string GetGroupValue(Buildingfootprint footprint, string propertyName)
    {
        var fieldName = $"_{propertyName.ToLowerInvariant()}";
        return ReadMember<string?>(footprint, propertyName, fieldName) ?? "Unknown";
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
