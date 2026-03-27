using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Module3.P2_5;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Domain.Module3.P2_5.Observers;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class CarbonChartControl : ICarbonChartControl
{
    private readonly IBuildingFootprintGateway _buildingFootprintGateway;
    private readonly IProductFootprintGateway _productFootprintGateway;
    private readonly IStaffFootprintGateway _staffFootprintGateway;
    private readonly IPackagingProfilerControl _packagingProfilerControl;

    public CarbonChartControl(
        IBuildingFootprintGateway buildingFootprintGateway,
        IProductFootprintGateway productFootprintGateway,
        IStaffFootprintGateway staffFootprintGateway,
        IPackagingProfilerControl packagingProfilerControl)
    {
        _buildingFootprintGateway = buildingFootprintGateway;
        _productFootprintGateway = productFootprintGateway;
        _staffFootprintGateway = staffFootprintGateway;
        _packagingProfilerControl = packagingProfilerControl;
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

    public CarbonDashboardDto BuildDashboardDto()
    {
        var productGraphData = _productFootprintGateway.GetProductGraphData();
        var buildingGraphData = CreateGraphs();
        var staffGraphData = _staffFootprintGateway.GetStaffGraphData();
        var packagingBarChart = BuildPackagingMaterialChart();
        var packagingTotal = Math.Round(packagingBarChart.Sum(item => item.Value), 2);
        Hotspots =
        [
            new ChartData("Product", Math.Round(productGraphData.Sum(item => item.Value), 2)),
            new ChartData("Building", Math.Round(buildingGraphData.Sum(item => item.Value), 2)),
            new ChartData("Staff", Math.Round(staffGraphData.Sum(item => item.Value), 2)),
            new ChartData("Packaging", packagingTotal)
        ];

        var hotspotThresholds = new List<ChartData>
        {
            new("Product", 1500.0),
            new("Building", 2000.0),
            new("Staff", 80.0),
            new("Packaging", 200.0)
        };

        var subject = new Subject(Hotspots, hotspotThresholds);
        var observer = new Observer();
        subject.AttachObserver(observer);
        subject.Evaluate();

        return new CarbonDashboardDto
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
            PackagingBarChart = packagingBarChart,
            PackagingPieChart = packagingBarChart,
            Hotspots = GetHotspots(),
            HotspotThresholds = hotspotThresholds,
            HotspotAlerts = observer.Alerts
        };
    }

    private List<ChartData> BuildPackagingMaterialChart()
    {
        var rows = _packagingProfilerControl.GetAllPackagingFootprints();
        if (rows == null || rows.Count == 0)
            return [];

        var totals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var name = (string)(row.MaterialName ?? "Unknown");
            var footprint = Convert.ToDouble(row.Footprintg);
            if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

            totals[name] = totals.TryGetValue(name, out var current)
                ? current + footprint
                : footprint;
        }

        return totals
            .Select(kvp => new ChartData(kvp.Key, Math.Round(kvp.Value, 2)))
            .OrderByDescending(item => item.Value)
            .ToList();
    }
}
