namespace ProRental.Domain.Entities;

public partial class Orderstatushistory
{
    // ── Public accessors ─────────────────────────────────────────────────
    public int HistoryId            => Historyid;
    public int OrderId              => Orderid;
    public DateTime OccurredAt      => _timestamp;   // Timestamp is taken by scaffolded private
    public string UpdatedBy         => Updatedby;    // Updatedby (lowercase b) ≠ UpdatedBy
    public string? StatusRemark     => _remark;      // Remark is taken by scaffolded private
}