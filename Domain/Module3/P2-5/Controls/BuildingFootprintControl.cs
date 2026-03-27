using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Entities.Module3;
using ProRental.Interfaces.Module3;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class BuildingFootprintControl : IBuildingFootprintControl
{
    private readonly IBuildingFootprintGateway _buildingGateway;
    private readonly BuildingFootprintStrategy _buildingFootprintStrategy;

    public BuildingFootprintControl(IBuildingFootprintGateway buildingGateway)
    {
        _buildingGateway = buildingGateway;
        _buildingFootprintStrategy = new BuildingFootprintStrategy();
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

    private void ValidateInputs(double roomSize, double co2Level, string zone, string block, string floor, string room)
    {
        if (roomSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(roomSize), "roomSize must be a positive number.");

        if (co2Level <= 0)
            throw new ArgumentOutOfRangeException(nameof(co2Level), "co2Level must be a positive number.");

        if (string.IsNullOrWhiteSpace(zone))
            throw new ArgumentException("zone must be one of: North, South, East, West, Central.", nameof(zone));

        if (string.IsNullOrWhiteSpace(floor))
            throw new ArgumentException("floor must be one of: Level 1, Level 2, Level 3, Level 4, Level 5.", nameof(floor));

        if (string.IsNullOrWhiteSpace(block))
            throw new ArgumentException("block cannot be empty.", nameof(block));

        if (string.IsNullOrWhiteSpace(room))
            throw new ArgumentException("room cannot be empty.", nameof(room));

        _buildingFootprintStrategy.CalculateFootprint(roomSize, co2Level, zone, floor);
    }

    private double CalculateTotalRoomCo2(double roomSize, double co2Level, string zone, string floor)
    {
        return _buildingFootprintStrategy.CalculateFootprint(roomSize, co2Level, zone, floor);
    }
}
