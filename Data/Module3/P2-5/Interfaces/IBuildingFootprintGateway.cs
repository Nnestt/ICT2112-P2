using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IBuildingFootprintGateway
{
    List<ChartData> GetHourlyChartData();
    List<ChartData> GetZoneGraphData();
    List<ChartData> GetHotspotData(string groupBy, int top = 5);
    Task<List<BuildingFootprintListItem>> GetBuildingFootprintsAsync();
    Task<Buildingfootprint> CreateBuildingFootprintAsync(Buildingfootprint footprint);
    Task<Buildingfootprint?> UpdateBuildingFootprintAsync(
        int buildingCarbonFootprintId,
        DateTime timehourly,
        string zone,
        string block,
        string floor,
        string room,
        double totalRoomCo2);
    Task<bool> DeleteBuildingFootprintAsync(int buildingCarbonFootprintId);
}

public sealed record BuildingFootprintListItem(
    int BuildingCarbonFootprintId,
    DateTime Timehourly,
    string Zone,
    string Block,
    string Floor,
    string Room,
    double TotalRoomCo2);
