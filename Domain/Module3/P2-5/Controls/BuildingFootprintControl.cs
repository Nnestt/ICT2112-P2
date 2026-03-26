using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class BuildingFootprintControl : IBuildingFootprintControl
{
    private const double CalibrationConstant = 0.000729;

    private static readonly IReadOnlyDictionary<string, double> ZoneWeights =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "North", 1.00 },
            { "South", 1.25 },
            { "East", 1.10 },
            { "West", 1.15 },
            { "Central", 1.35 }
        };

    private static readonly IReadOnlyDictionary<string, double> FloorWeights =
        new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "Level 1", 1.00 },
            { "Level 2", 1.20 },
            { "Level 3", 1.45 },
            { "Level 4", 1.60 },
            { "Level 5", 1.75 }
        };

    private readonly IBuildingFootprintGateway _buildingGateway;

    public BuildingFootprintControl(IBuildingFootprintGateway buildingGateway)
    {
        _buildingGateway = buildingGateway;
    }

    public Task<List<BuildingFootprintListItem>> GetBuildingFootprintsAsync()
    {
        return _buildingGateway.GetBuildingFootprintsAsync();
    }

    public async Task<Buildingfootprint> CreateBuildingFootprintAsync(
        double roomSize,
        double co2Level,
        string zone,
        string block,
        string floor,
        string room)
    {
        ValidateInputs(roomSize, co2Level, zone, block, floor, room);
        var totalRoomCo2 = CalculateTotalRoomCo2(roomSize, co2Level, zone, floor);

        var footprint = Buildingfootprint.Create(
            DateTime.UtcNow,
            zone,
            block,
            floor,
            room,
            totalRoomCo2);

        return await _buildingGateway.CreateBuildingFootprintAsync(footprint);
    }

    public Task<Buildingfootprint?> UpdateBuildingFootprintAsync(
        int buildingCarbonFootprintId,
        double roomSize,
        double co2Level,
        string zone,
        string block,
        string floor,
        string room)
    {
        if (buildingCarbonFootprintId <= 0)
            throw new ArgumentOutOfRangeException(nameof(buildingCarbonFootprintId), "buildingCarbonFootprintId must be a positive integer.");

        ValidateInputs(roomSize, co2Level, zone, block, floor, room);
        var totalRoomCo2 = CalculateTotalRoomCo2(roomSize, co2Level, zone, floor);

        return _buildingGateway.UpdateBuildingFootprintAsync(
            buildingCarbonFootprintId,
            DateTime.UtcNow,
            zone,
            block,
            floor,
            room,
            totalRoomCo2);
    }

    public Task<bool> DeleteBuildingFootprintAsync(int buildingCarbonFootprintId)
    {
        if (buildingCarbonFootprintId <= 0)
            throw new ArgumentOutOfRangeException(nameof(buildingCarbonFootprintId), "buildingCarbonFootprintId must be a positive integer.");

        return _buildingGateway.DeleteBuildingFootprintAsync(buildingCarbonFootprintId);
    }

    private static void ValidateInputs(double roomSize, double co2Level, string zone, string block, string floor, string room)
    {
        if (roomSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(roomSize), "roomSize must be a positive number.");

        if (co2Level <= 0)
            throw new ArgumentOutOfRangeException(nameof(co2Level), "co2Level must be a positive number.");

        if (string.IsNullOrWhiteSpace(zone) || !ZoneWeights.ContainsKey(zone))
            throw new ArgumentException("zone must be one of: North, South, East, West, Central.", nameof(zone));

        if (string.IsNullOrWhiteSpace(floor) || !FloorWeights.ContainsKey(floor))
            throw new ArgumentException("floor must be one of: Level 1, Level 2, Level 3, Level 4, Level 5.", nameof(floor));

        if (string.IsNullOrWhiteSpace(block))
            throw new ArgumentException("block cannot be empty.", nameof(block));

        if (string.IsNullOrWhiteSpace(room))
            throw new ArgumentException("room cannot be empty.", nameof(room));
    }

    private static double CalculateTotalRoomCo2(double roomSize, double co2Level, string zone, string floor)
    {
        var totalRoomCo2 = roomSize
            * co2Level
            * ZoneWeights[zone]
            * FloorWeights[floor]
            * CalibrationConstant;

        return Math.Round(totalRoomCo2, 2);
    }
}
