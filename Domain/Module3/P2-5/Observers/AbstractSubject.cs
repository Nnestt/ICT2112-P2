using ProRental.Domain.Module3.P2_5.Entities;

namespace ProRental.Domain.Module3.P2_5.Observers;

public abstract class AbstractSubject
{
    private readonly List<IObserver> _observers = [];

    public void AttachObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void DetachObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void Notify(List<ChartData> hotspotData, List<ChartData> thresholdData)
    {
        foreach (var observer in _observers)
        {
            observer.Update(hotspotData, thresholdData);
        }
    }
}
