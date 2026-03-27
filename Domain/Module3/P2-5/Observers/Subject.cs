using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Domain.Module3.P2_5.Observers;

public sealed class Subject : AbstractSubject
{
    private readonly List<ChartData> _hotspotData;
    private readonly List<ChartData> _thresholdData;

    public Subject(List<ChartData> hotspotData, List<ChartData> thresholdData)
    {
        _hotspotData = hotspotData;
        _thresholdData = thresholdData;
    }

    public void Evaluate()
    {
        Notify(_hotspotData, _thresholdData);
    }
}
