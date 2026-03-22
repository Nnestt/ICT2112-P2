using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class CarbonChartControl : ICarbonChartService
{
    private readonly IBuildingFootprintGateway _buildingFootprintGateway;
    private readonly IProductFootprintGateway _productFootprintGateway;
    private readonly IStaffFootprintGateway _staffFootprintGateway;

    public CarbonChartControl(
        IBuildingFootprintGateway buildingFootprintGateway,
        IProductFootprintGateway productFootprintGateway,
        IStaffFootprintGateway staffFootprintGateway)
    {
        _buildingFootprintGateway = buildingFootprintGateway;
        _productFootprintGateway = productFootprintGateway;
        _staffFootprintGateway = staffFootprintGateway;
    }

    public List<ChartData> Hotspots { get; private set; } = [];

    public List<ChartData> CreateCharts()
    {
        return _buildingFootprintGateway.GetHourlyChartData();
    }

    public List<ChartData> CreateGraphs()
    {
        return _buildingFootprintGateway.GetZoneGraphData();
    }

    public void IdentifyHotspots(string groupBy)
    {
        Hotspots = _buildingFootprintGateway.GetHotspotData(groupBy);
    }

    public List<ChartData> GetHotspots()
    {
        return Hotspots;
    }

    public CarbonDashboardViewModel BuildDashboardViewModel()
    {
        var productGraphData = _productFootprintGateway.GetProductGraphData();
        var buildingGraphData = CreateGraphs();
        var staffGraphData = _staffFootprintGateway.GetStaffGraphData();
        Hotspots =
        [
            new ChartData("Product", Math.Round(productGraphData.Sum(item => item.Value), 2)),
            new ChartData("Building", Math.Round(buildingGraphData.Sum(item => item.Value), 2)),
            new ChartData("Staff", Math.Round(staffGraphData.Sum(item => item.Value), 2))
        ];

        return new CarbonDashboardViewModel
        {
            ProductTrendline = _productFootprintGateway.GetHourlyChartData(),
            ProductBarChart = productGraphData,
            ProductPieChart = productGraphData,
            BuildingTrendline = CreateCharts(),
            BuildingBarChart = buildingGraphData,
            BuildingPieChart = buildingGraphData,
            StaffTrendline = _staffFootprintGateway.GetHourlyChartData(),
            StaffBarChart = staffGraphData,
            StaffPieChart = staffGraphData,
            Hotspots = GetHotspots()
        };
    }
}
