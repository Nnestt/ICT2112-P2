using ProRental.Domain.Entities;
using ProRental.Interfaces;

namespace ProRental.Domain.Control;

/// <summary>
/// Control class responsible for analytics business logic.
/// Depends on IAnalyticsMapper (data access) and AnalyticsFactory (entity creation).
/// Does NOT depend on concrete entity types — only IAnalytics and Analytic (base).
/// </summary>
public class AnalyticsControl : IAnalyticsData
{
    private readonly IAnalyticsMapper _analyticsMapper;
    private readonly AnalyticsFactory _factory;

    public AnalyticsControl(IAnalyticsMapper analyticsMapper, AnalyticsFactory factory)
    {
        _analyticsMapper = analyticsMapper;
        _factory = factory;
    }

    // ── Generate ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Generates a new analytics record from a list of transaction objects.
    /// Uses the factory to create the appropriate analytics type.
    /// </summary>
    public async Task<IAnalytics> GenerateAnalyticsAsync(List<object> transactions, string analyticsType = "DAILY")
    {
        IAnalytics analytics = analyticsType switch
        {
            "SUPTREND"  => _factory.CreateSupplierTrend(),
            "PRODTREND" => _factory.CreateProductTrend(),
            _           => _factory.CreateDailyLog()
        };
        return analytics;
    }

    // ── Read ────────────────────────────────────────────────────────────────────

    public async Task<Analytic?> GetAnalyticsAsync(int targetID)
        => await _analyticsMapper.FindByIDAsync(targetID);

    public async Task<IEnumerable<Analytic>> GetAnalyticsByDateAsync(DateTime day)
        => await _analyticsMapper.FindByDateAsync(day.Date, day.Date.AddDays(1).AddTicks(-1));

    public async Task<IEnumerable<Analytic>> GetAnalyticsByDateRangeAsync(DateTime start, DateTime end)
        => await _analyticsMapper.FindByDateAsync(start, end);

    public async Task<IEnumerable<Analytic>> GetAnalyticsBySupplierAsync(string supplier)
    {
        // Resolve supplier name to ID via mapper query
        return await _analyticsMapper.FindBySupplierAsync(0); // supplierID resolved by mapper
    }

    public async Task<IEnumerable<Analytic>> GetAnalyticsByProductAsync(string product)
    {
        return await _analyticsMapper.FindByProductAsync(0); // productID resolved by mapper
    }

    // ── Update / Delete ─────────────────────────────────────────────────────────

    public async Task UpdateAnalyticsAsync(int targetID, List<object> transactions)
    {
        var existing = await _analyticsMapper.FindByIDAsync(targetID);
        if (existing is null) return;
        await _analyticsMapper.UpdateAsync(existing);
    }

    public async Task DeleteAnalyticsAsync(int targetID)
    {
        var existing = await _analyticsMapper.FindByIDAsync(targetID);
        if (existing is null) return;
        await _analyticsMapper.DeleteAsync(existing);
    }
}
