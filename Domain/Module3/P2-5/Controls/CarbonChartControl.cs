using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class CarbonChartControl : ICarbonChartService
{
    private readonly IBuildingFootprintTableGateway _buildingFootprintTableGateway;

    public CarbonChartControl(IBuildingFootprintTableGateway buildingFootprintTableGateway)
    {
        _buildingFootprintTableGateway = buildingFootprintTableGateway;
    }

    public List<ChartData> Hotspots { get; private set; } = [];

    public List<ChartData> CreateCharts()
    {
        return _buildingFootprintTableGateway.GetHourlyChartData();
    }

    public List<ChartData> CreateGraphs()
    {
        return _buildingFootprintTableGateway.GetZoneGraphData();
    }

    public void IdentifyHotspots(string groupBy)
    {
        Hotspots = _buildingFootprintTableGateway.GetHotspotData(groupBy);
    }

    public List<ChartData> GetHotspots()
    {
        return Hotspots;
    }

    public CarbonDashboardViewModel BuildDashboardViewModel()
    {
        IdentifyHotspots("room");

        var buildingGraphData = CreateGraphs();

        return new CarbonDashboardViewModel
        {
            BuildingTrendline = CreateCharts(),
            BuildingBarChart = buildingGraphData,
            BuildingPieChart = buildingGraphData,
            Hotspots = GetHotspots()
        };
    }
}
