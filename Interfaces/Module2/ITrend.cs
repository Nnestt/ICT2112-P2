namespace ProRental.Interfaces;

/// <summary>
/// Segregated interface for trend-specific queries.
/// Consumers that only need trend data depend on this, not IAnalyticsData.
/// </summary>
public interface ITrend
{
    Task<float?> GetSupplierReliabilityAsync(int targetID);
    Task<float?> GetTurnoverRateAsync(int targetID);
}
