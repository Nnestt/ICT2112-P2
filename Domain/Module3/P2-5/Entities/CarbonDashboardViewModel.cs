namespace ProRental.Domain.Module3.P2_5.Entities;

public sealed class CarbonDashboardViewModel
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
    public List<ChartData> Hotspots { get; init; } = [];
}
