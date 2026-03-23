using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IStaffFootprintGateway
{
    List<ChartData> GetHourlyChartData();
    List<ChartData> GetStaffGraphData();
    List<ChartData> GetHotspotData(int top = 5);
}
