using ProRental.Domain.Entities;

namespace ProRental.Interfaces;

/// <summary>
/// Data mapper interface for Analytics persistence.
/// Implemented by AnalysisRecordMapper in the Data layer.
/// </summary>
public interface IAnalyticsMapper
{
    Task<bool> InsertAsync(Analytic analytics);
    Task<Analytic?> FindByIDAsync(int id);
    Task<IEnumerable<Analytic>> FindByDateAsync(DateTime start, DateTime end);
    Task<IEnumerable<Analytic>> FindBySupplierAsync(int supplierID);
    Task<IEnumerable<Analytic>> FindByProductAsync(int productID);
    Task<bool> UpdateAsync(Analytic analytics);
    Task<bool> DeleteAsync(Analytic analytics);
}
