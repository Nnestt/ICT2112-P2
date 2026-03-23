using ProRental.Domain.Entities;
using ProRental.Interfaces;

namespace ProRental.Controllers;

/// <summary>
/// ViewModel for Analytics Index page — bundles analytics list + filter state.
/// </summary>
public class AnalyticsIndexViewModel
{
    public IEnumerable<Analytic> Analytics { get; set; } = [];
    public string? FilterType       { get; set; }
    public string? FilterSupplier   { get; set; }
    public string? FilterProduct    { get; set; }
    public DateTime? FilterStart    { get; set; }
    public DateTime? FilterEnd      { get; set; }
}

/// <summary>
/// ViewModel for Analytics Details page.
/// </summary>
public class AnalyticsDetailsViewModel
{
    public Analytic Analytic { get; set; } = null!;
    public IEnumerable<TransactionLogDto> TransactionLogs { get; set; } = [];
    public Reportexport? ExistingReport { get; set; }   // null = show Generate form
}
