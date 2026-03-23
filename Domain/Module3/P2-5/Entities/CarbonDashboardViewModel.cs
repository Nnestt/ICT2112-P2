namespace ProRental.Domain.Module3.P2_5.Entities;

public sealed class CarbonDashboardViewModel
{
    private List<ChartData> _productTrendline = [];
    private List<ChartData> _productBarChart = [];
    private List<ChartData> _productPieChart = [];
    private List<ChartData> _buildingTrendline = [];
    private List<ChartData> _buildingBarChart = [];
    private List<ChartData> _buildingPieChart = [];
    private List<ChartData> _staffTrendline = [];
    private List<ChartData> _staffBarChart = [];
    private List<ChartData> _staffPieChart = [];
    private List<ChartData> _hotspots = [];
    private List<ChartData> _hotspotThresholds = [];
    private List<string> _hotspotAlerts = [];

    public List<ChartData> ProductTrendline
    {
        get => _productTrendline;
        init => _productTrendline = value;
    }

    public List<ChartData> ProductBarChart
    {
        get => _productBarChart;
        init => _productBarChart = value;
    }

    public List<ChartData> ProductPieChart
    {
        get => _productPieChart;
        init => _productPieChart = value;
    }

    public List<ChartData> BuildingTrendline
    {
        get => _buildingTrendline;
        init => _buildingTrendline = value;
    }

    public List<ChartData> BuildingBarChart
    {
        get => _buildingBarChart;
        init => _buildingBarChart = value;
    }

    public List<ChartData> BuildingPieChart
    {
        get => _buildingPieChart;
        init => _buildingPieChart = value;
    }

    public List<ChartData> StaffTrendline
    {
        get => _staffTrendline;
        init => _staffTrendline = value;
    }

    public List<ChartData> StaffBarChart
    {
        get => _staffBarChart;
        init => _staffBarChart = value;
    }

    public List<ChartData> StaffPieChart
    {
        get => _staffPieChart;
        init => _staffPieChart = value;
    }

    public List<ChartData> Hotspots
    {
        get => _hotspots;
        init => _hotspots = value;
    }

    public List<ChartData> HotspotThresholds
    {
        get => _hotspotThresholds;
        init => _hotspotThresholds = value;
    }

    public List<string> HotspotAlerts
    {
        get => _hotspotAlerts;
        init => _hotspotAlerts = value;
    }
}
