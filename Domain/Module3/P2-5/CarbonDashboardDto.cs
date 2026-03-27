using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Domain.Module3.P2_5;

public sealed class CarbonDashboardDto
{
    public List<ChartData> ProductTrendline { get; init; } = [];
    public List<ChartData> ProductBarChart { get; init; } = [];
    public List<ChartData> ProductPieChart { get; init; } = [];
    public List<ChartData> BuildingTrendline { get; init; } = [];
    public List<ChartData> BuildingBarChart { get; init; } = [];
    public List<ChartData> BuildingPieChart { get; init; } = [];
    public List<ChartData> StaffTrendline { get; init; } = [];
    public List<ChartData> StaffBarChart { get; init; } = [];
    public List<ChartData> StaffPieChart { get; init; } = [];
    public List<ChartData> PackagingBarChart { get; init; } = [];
    public List<ChartData> PackagingPieChart { get; init; } = [];
    public List<ChartData> Hotspots { get; init; } = [];
    public List<ChartData> HotspotThresholds { get; init; } = [];
    public List<string> HotspotAlerts { get; init; } = [];
}
