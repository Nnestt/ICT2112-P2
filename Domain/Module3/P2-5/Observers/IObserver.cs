using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Domain.Module3.P2_5.Observers;

public interface IObserver
{
    void Update(List<ChartData> hotspotData, List<ChartData> thresholdData);
}
