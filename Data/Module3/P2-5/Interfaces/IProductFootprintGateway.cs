using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IProductFootprintGateway
{
    List<ChartData> GetHourlyChartData();
    List<ChartData> GetProductGraphData();
    List<ChartData> GetHotspotData(int top = 5);
}
