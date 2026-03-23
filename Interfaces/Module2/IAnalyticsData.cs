using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

/// <summary>
/// Interface for reading analytics records.
/// Consumers depending on core analytics queries use this interface only.
/// </summary>
public interface IAnalyticsData
{
    Task<Analytic?> GetAnalyticsAsync(int targetID);
    Task<IEnumerable<Analytic>> GetAnalyticsByDateAsync(DateTime day);
    Task<IEnumerable<Analytic>> GetAnalyticsByDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<Analytic>> GetAnalyticsBySupplierAsync(string supplier);
    Task<IEnumerable<Analytic>> GetAnalyticsByProductAsync(string product);
}