using ProRental.Domain.Entities;
using ProRental.Data.Module3.P2_5.Interfaces;

namespace ProRental.Interfaces.Module3.P2_5;

public interface IBuildingFootprintControl
{
    Task<Buildingfootprint> CreateBuildingFootprintAsync(
        double roomSize,
        double co2Level,
        string zone,
        string block,
        string floor,
        string room);

    Task<List<BuildingFootprintListItem>> GetBuildingFootprintsAsync();

    Task<Buildingfootprint?> UpdateBuildingFootprintAsync(
        int buildingCarbonFootprintId,
        double roomSize,
        double co2Level,
        string zone,
        string block,
        string floor,
        string room);

    Task<bool> DeleteBuildingFootprintAsync(int buildingCarbonFootprintId);
}
