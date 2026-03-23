using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Interfaces;

namespace ProRental.Data.Gateways;

/// <summary>
/// Reads from TransactionLog table and its 5 child log tables.
/// Uses raw SQL to retrieve logtype since it's not in the EF scaffold.
/// </summary>
public class TransactionLogService : ITransactionLogService
{
    private readonly AppDbContext _db;

    public TransactionLogService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<TransactionLogDto>> GetAllTransactionLogsAsync()
        => await FetchLogsAsync();

    public async Task<IEnumerable<TransactionLogDto>> GetTransactionLogsByDateRangeAsync(
        DateTime start, DateTime end)
        => (await FetchLogsAsync())
            .Where(t => t.CreatedAt >= start && t.CreatedAt <= end);

    private async Task<List<TransactionLogDto>> FetchLogsAsync()
    {
        // Raw SQL to get logtype since scaffold doesn't expose it
        var results = await _db.Database
            .SqlQueryRaw<TransactionLogRaw>(
                "SELECT transactionlogid AS Id, logtype AS LogType, createdat AS CreatedAt FROM transactionlog")
            .ToListAsync();

        return results.Select(r => new TransactionLogDto
        {
            LogID     = r.Id,
            LogType   = r.LogType ?? "UNKNOWN",
            CreatedAt = r.CreatedAt ?? DateTime.UtcNow,
            Summary   = $"{r.LogType ?? "LOG"} #{r.Id}"
        }).ToList();
    }
}

/// <summary>
/// Internal projection class for raw SQL query result.
/// </summary>
internal class TransactionLogRaw
{
    public int Id { get; set; }
    public string? LogType { get; set; }
    public DateTime? CreatedAt { get; set; }
}