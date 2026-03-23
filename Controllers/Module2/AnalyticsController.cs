using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Control;
using ProRental.Domain.Enums;
using ProRental.Interfaces;

namespace ProRental.Controllers;

public class AnalyticsController : Controller
{
    private readonly AnalyticsControl      _analyticsControl;
    private readonly ReportExportControl   _reportControl;

    private const string IndexView   = "~/Views/Module2/Analytics/Index.cshtml";
    private const string DetailView  = "~/Views/Module2/Analytics/Details.cshtml";
    private const string ReportView  = "~/Views/Module2/Analytics/Report.cshtml";
    private const string CreateView  = "~/Views/Module2/Analytics/Create.cshtml";

    public AnalyticsController(AnalyticsControl analyticsControl, ReportExportControl reportControl)
    {
        _analyticsControl = analyticsControl;
        _reportControl    = reportControl;
    }

    // ── Index ─────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Index(
        string? type, string? supplier, string? product,
        DateTime? start, DateTime? end)
    {
        // Default date range: today in SGT (+08), widened ±1 day for timezone safety
        var now       = DateTime.UtcNow.AddHours(8);
        var startDate = (start ?? now).Date.AddDays(-1).ToUniversalTime();
        var endDate   = (end   ?? now).Date.AddDays(1).AddTicks(-1).ToUniversalTime();

        IEnumerable<ProRental.Domain.Entities.Analytic> analytics;

        if (!string.IsNullOrWhiteSpace(supplier))
            analytics = await _analyticsControl.GetAnalyticsBySupplierAsync(supplier);
        else if (!string.IsNullOrWhiteSpace(product))
            analytics = await _analyticsControl.GetAnalyticsByProductAsync(product);
        else if (start.HasValue || end.HasValue)
            analytics = await _analyticsControl.GetAnalyticsByDateRangeAsync(startDate, endDate);
        else
            analytics = await _analyticsControl.GetAllAnalyticsAsync();

        // Filter by type client-side
        if (!string.IsNullOrWhiteSpace(type) && type != "ALL")
            analytics = analytics.Where(a => a.GetAnalyticsType() == type);

        var vm = new AnalyticsIndexViewModel
        {
            Analytics      = analytics,
            FilterType     = type,
            FilterSupplier = supplier,
            FilterProduct  = product,
            FilterStart    = start ?? now,
            FilterEnd      = end   ?? now,
        };

        return View(IndexView, vm);
    }

    // ── Details ───────────────────────────────────────────────────────────────

    public async Task<IActionResult> Details(int id)
    {
        var analytic = await _analyticsControl.GetAnalyticsAsync(id);
        if (analytic is null) return NotFound();

        var logs = await _analyticsControl.GetLogsForAnalyticsAsync(analytic);

        // Check if a report already exists for this analytics record
        var allReports   = await _reportControl.GetAllReportsAsync();
        var existingReport = allReports.FirstOrDefault(r => r.GetRefAnalyticsID() == id);

        var vm = new AnalyticsDetailsViewModel
        {
            Analytic        = analytic,
            TransactionLogs = logs,
            ExistingReport  = existingReport
        };

        return View(DetailView, vm);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Create()
    {
        var logs = await _analyticsControl.GetAllLogsAsync();
        return View(CreateView, logs);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        string analyticsType, DateTime startDate, DateTime endDate, string refPrimaryName)
    {
        // Widen for timezone safety
        var start = startDate.Date.AddDays(-1).ToUniversalTime();
        var end   = endDate.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

        var analytic = await _analyticsControl.CreateAnalyticsAsync(
            analyticsType, start, end, refPrimaryName);

        return RedirectToAction(nameof(Details), new { id = analytic.GetID() });
    }

    // ── Report actions ────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> GenerateReport(
        int refAnalyticsID, string title, VisualType visualType, FileFormat fileFormat)
    {
        await _reportControl.GenerateReportAsync(refAnalyticsID, title, visualType, fileFormat);
        return RedirectToAction(nameof(Details), new { id = refAnalyticsID });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReport(
        int id, int refAnalyticsID, string title, VisualType visualType, FileFormat fileFormat)
    {
        await _reportControl.UpdateReportAsync(id, title, visualType, fileFormat);
        return RedirectToAction(nameof(Details), new { id = refAnalyticsID });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReport(int id, int refAnalyticsID)
    {
        var report = await _reportControl.GetReportAsync(id);
        if (report is not null) await _reportControl.DeleteReportAsync(report);
        return RedirectToAction(nameof(Details), new { id = refAnalyticsID });
    }

    // ── Export (hardcoded dummy file) ─────────────────────────────────────────

    public async Task<IActionResult> ExportReport(int id)
    {
        var report = await _reportControl.GetReportAsync(id);
        if (report is null) return NotFound();

        var format = report.GetFileFormat();
        if (format == FileFormat.CSV)
        {
            var csv = "AnalyticsID,Title,VisualType\n" +
                      $"{report.GetRefAnalyticsID()},{report.GetTitle()},{report.GetVisualType()}\n";
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"{report.GetTitle()}.csv");
        }
        else if (format == FileFormat.XLSX)
        {
            // Dummy XLSX — return CSV bytes with xlsx extension for presentation
            var csv = "AnalyticsID,Title,VisualType\n" +
                      $"{report.GetRefAnalyticsID()},{report.GetTitle()},{report.GetVisualType()}\n";
            return File(System.Text.Encoding.UTF8.GetBytes(csv),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{report.GetTitle()}.xlsx");
        }
        else
        {
            // PDF — return dummy text as PDF placeholder
            var content = $"Report: {report.GetTitle()}\nAnalytics ID: {report.GetRefAnalyticsID()}\nGenerated: {DateTime.Now}";
            return File(System.Text.Encoding.UTF8.GetBytes(content), "application/pdf", $"{report.GetTitle()}.pdf");
        }
    }

    // ── Report page (standalone) ──────────────────────────────────────────────

    public async Task<IActionResult> Report(int id)
    {
        var report = await _reportControl.GetReportAsync(id);
        if (report is null) return NotFound();
        return View(ReportView, report);
    }
}