using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

/// <summary>
/// Data mapper interface for ReportExport persistence.
/// Implemented by ReportMapper in the Data layer.
/// </summary>
public interface IReportExportMapper
{
    Task<bool> InsertAsync(Reportexport report);
    Task<Reportexport?> FindByIDAsync(int id);
    Task<Reportexport?> FindByTitleAsync(string title);
    Task<bool> UpdateAsync(Reportexport report);
    Task<bool> DeleteAsync(Reportexport report);
}
