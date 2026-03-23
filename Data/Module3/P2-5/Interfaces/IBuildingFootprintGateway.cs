using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IBuildingFootprintGateway
{
    List<ChartData> GetHourlyChartData();
    List<ChartData> GetZoneGraphData();
    List<ChartData> GetHotspotData(string groupBy, int top = 5);
}
