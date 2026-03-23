using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Domain.Module3.P2_5.Observers;

public sealed class Observer : IObserver
{
    public List<string> Alerts { get; } = [];

    public void Update(List<ChartData> hotspotData, List<ChartData> thresholdData)
    {
        Alerts.Clear();

        var thresholdMap = thresholdData.ToDictionary(item => item.Label, item => item.Value);

        foreach (var hotspot in hotspotData)
        {
            if (thresholdMap.TryGetValue(hotspot.Label, out var threshold) && hotspot.Value >= threshold)
            {
                Alerts.Add($"{hotspot.Label} reached {hotspot.Value:F2} CO2 (threshold: {threshold:F2}).");
            }
        }
    }
}
