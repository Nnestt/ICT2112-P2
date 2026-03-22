namespace ProRental.Interfaces;

/// <summary>
/// Common contract for all Analytics product types returned by AnalyticsFactory.
/// AnalyticsControl depends on this interface, never on concrete entity types.
/// </summary>
public interface IAnalytics
{
    string GetType();
    int GetID();
}
