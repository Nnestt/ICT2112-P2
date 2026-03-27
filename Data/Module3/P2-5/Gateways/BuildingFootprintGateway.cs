using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Module3.P2_5.Entities;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Buildingfootprint> CreateBuildingFootprintAsync(Buildingfootprint footprint)
    {
        _dbContext.Buildingfootprints.Add(footprint);
        await _dbContext.SaveChangesAsync();
        return footprint;
    }

    public Task<List<BuildingFootprintListItem>> GetBuildingFootprintsAsync()
    {
        var items = _dbContext.Buildingfootprints
            .AsEnumerable()
            .Select(footprint => new BuildingFootprintListItem(
                ReadMember<int>(footprint, "Buildingcarbonfootprintid", "_buildingcarbonfootprintid"),
                GetTimeHourly(footprint),
                GetGroupValue(footprint, "Zone"),
                GetGroupValue(footprint, "Block"),
                GetGroupValue(footprint, "Floor"),
                GetGroupValue(footprint, "Room"),
                GetTotalRoomCo2(footprint)))
            .OrderByDescending(item => item.Timehourly)
            .ThenByDescending(item => item.BuildingCarbonFootprintId)
            .ToList();

        return Task.FromResult(items);
    }

    public async Task<Buildingfootprint?> UpdateBuildingFootprintAsync(
        int buildingCarbonFootprintId,
        DateTime timehourly,
        string zone,
        string block,
        string floor,
        string room,
        double totalRoomCo2)
    {
        var footprint = await _dbContext.Buildingfootprints
            .FirstOrDefaultAsync(item => EF.Property<int>(item, "Buildingcarbonfootprintid") == buildingCarbonFootprintId);

        if (footprint == null)
        {
            return null;
        }

        WriteMember(footprint, "Timehourly", "_timehourly", timehourly);
        WriteMember(footprint, "Zone", "_zone", zone);
        WriteMember(footprint, "Block", "_block", block);
        WriteMember(footprint, "Floor", "_floor", floor);
        WriteMember(footprint, "Room", "_room", room);
        WriteMember(footprint, "Totalroomco2", "_totalroomco2", totalRoomCo2);

        await _dbContext.SaveChangesAsync();
        return footprint;
    }

    public async Task<bool> DeleteBuildingFootprintAsync(int buildingCarbonFootprintId)
    {
        var footprint = await _dbContext.Buildingfootprints
            .FirstOrDefaultAsync(item => EF.Property<int>(item, "Buildingcarbonfootprintid") == buildingCarbonFootprintId);

        if (footprint == null)
        {
            return false;
        }

        _dbContext.Buildingfootprints.Remove(footprint);
        await _dbContext.SaveChangesAsync();
        return true;
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

    private static void WriteMember<T>(object target, string propertyName, string fieldName, T value)
    {
        var type = target.GetType();

        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null)
        {
            property.SetValue(target, value);
            return;
        }

        var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        throw new InvalidOperationException($"Unable to write '{propertyName}' on {type.Name}.");
    }
}
