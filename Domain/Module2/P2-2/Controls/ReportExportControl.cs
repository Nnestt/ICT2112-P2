using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces;

namespace ProRental.Domain.Control;

/// <summary>
/// Control class for report export operations.
/// Separated from AnalyticsControl per SRP — report generation is a distinct responsibility.
/// </summary>
public class ReportExportControl
{
    private readonly IReportExportMapper _reportMapper;

    public ReportExportControl(IReportExportMapper reportMapper)
    {
        _reportMapper = reportMapper;
    }

    /// <summary>
    /// Creates and persists a new report export linked to an analytics record.
    /// visualType and fileFormat are optional and default to TABLE / PDF.
    /// </summary>
    public async Task GenerateReportAsync(
        int refAnalyticsID,
        string title,
        VisualType? visualType = null,
        FileFormat? fileFormat = null)
    {
        var report = new Reportexport
        {
            Refanalyticsid = refAnalyticsID,
            Title          = title,
        };

        report.UpdateType(visualType ?? VisualType.Table);
        report.UpdateFormat(fileFormat ?? FileFormat.Pdf);

        await _reportMapper.InsertAsync(report);
    }

    public async Task<Reportexport?> GetReportAsync(int targetID)
        => await _reportMapper.FindByIDAsync(targetID);

    public async Task UpdateReportAsync(
        int targetID,
        string title,
        VisualType visualType,
        FileFormat fileFormat)
    {
        var report = await _reportMapper.FindByIDAsync(targetID);
        if (report is null) return;

        report.Title = title;
        report.UpdateType(visualType);
        report.UpdateFormat(fileFormat);

        await _reportMapper.UpdateAsync(report);
    }

    public async Task DeleteReportAsync(Reportexport report)
        => await _reportMapper.DeleteAsync(report);
}
