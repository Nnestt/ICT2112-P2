using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces;

namespace ProRental.Domain.Control;

/// <summary>
/// Factory Pattern — centralises creation of Analytics product types.
/// AnalyticsControl calls this factory instead of instantiating concrete types directly,
/// decoupling the control from DailyLog, SupplierTrend, and ProductTrend.
/// </summary>
public class AnalyticsFactory
{
    /// <summary>
    /// Creates a DailyLog analytics record (base daily transactional data).
    /// </summary>
    public IAnalytics CreateDailyLog()
    {
        var entity = new Analytic();
        entity.UpdateType(AnalyticsType.DAILY);
        return new DailyLogAnalytics(entity);
    }

    /// <summary>
    /// Creates a SupplierTrend analytics record (derived supplier reliability metrics).
    /// </summary>
    public IAnalytics CreateSupplierTrend()
    {
        var entity = new SupplierTrend();
        entity.UpdateType(AnalyticsType.SUPTREND);
        return new SupplierTrendAnalytics(entity);
    }

    /// <summary>
    /// Creates a ProductTrend analytics record (derived product turnover metrics).
    /// </summary>
    public IAnalytics CreateProductTrend()
    {
        var entity = new ProductTrend();
        entity.UpdateType(AnalyticsType.PRODTREND);
        return new ProductTrendAnalytics(entity);
    }
}

// ── Wrappers — adapt concrete entities to IAnalytics ──────────────────────────

/// <summary>Adapts Analytic (DailyLog) to IAnalytics interface.</summary>
internal class DailyLogAnalytics : IAnalytics
{
    private readonly Analytic _entity;
    public DailyLogAnalytics(Analytic entity) => _entity = entity;
    public string GetType() => "DAILY";
    public int GetID() => _entity.Analyticsid;
}

/// <summary>Adapts SupplierTrend to IAnalytics interface.</summary>
internal class SupplierTrendAnalytics : IAnalytics
{
    private readonly SupplierTrend _entity;
    public SupplierTrendAnalytics(SupplierTrend entity) => _entity = entity;
    public string GetType() => "SUPTREND";
    public int GetID() => _entity.Analyticsid;
}

/// <summary>Adapts ProductTrend to IAnalytics interface.</summary>
internal class ProductTrendAnalytics : IAnalytics
{
    private readonly ProductTrend _entity;
    public ProductTrendAnalytics(ProductTrend entity) => _entity = entity;
    public string GetType() => "PRODTREND";
    public int GetID() => _entity.Analyticsid;
}
