using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

public interface IReportExportMapper
{
    Task<bool> InsertAsync(Reportexport report);
    Task<Reportexport?> FindByIDAsync(int id);
    Task<Reportexport?> FindByTitleAsync(string title);
    Task<IEnumerable<Reportexport>> GetAllAsync();
    Task<bool> UpdateAsync(Reportexport report);
    Task<bool> DeleteAsync(Reportexport report);
}