using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Interfaces.Module3.P2_5;

public interface ICarbonChartService
{
    List<ChartData> CreateCharts();
    List<ChartData> CreateGraphs();
    void IdentifyHotspots(string groupBy);
    List<ChartData> GetHotspots();
    CarbonDashboardViewModel BuildDashboardViewModel();
}
