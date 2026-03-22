using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces;

namespace ProRental.Data.Gateways;

/// <summary>
/// Data mapper for the Analytics table.
/// Implements IAnalyticsMapper — the control layer only depends on the interface.
/// </summary>
public class AnalysisRecordMapper : IAnalyticsMapper
{
    private readonly AppDbContext _db;

    public AnalysisRecordMapper(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> InsertAsync(Analytic analytics)
    {
        _db.Analytics.Add(analytics);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<Analytic?> FindByIDAsync(int id)
        => await _db.Analytics.FindAsync(id);

    public async Task<IEnumerable<Analytic>> FindByDateAsync(DateTime start, DateTime end)
        => await _db.Analytics
            .Where(a => a.Startdate >= start && a.Enddate <= end)
            .ToListAsync();

    public async Task<IEnumerable<Analytic>> FindBySupplierAsync(int supplierID)
        => await _db.Analytics
            .Where(a => a.Analyticstype == AnalyticsType.Suptrend
                     && a.Refprimaryid == supplierID)
            .ToListAsync();

    public async Task<IEnumerable<Analytic>> FindByProductAsync(int productID)
        => await _db.Analytics
            .Where(a => a.Analyticstype == AnalyticsType.Prodtrend
                     && a.Refprimaryid == productID)
            .ToListAsync();

    public async Task<bool> UpdateAsync(Analytic analytics)
    {
        _db.Analytics.Update(analytics);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Analytic analytics)
    {
        _db.Analytics.Remove(analytics);
        return await _db.SaveChangesAsync() > 0;
    }
}
