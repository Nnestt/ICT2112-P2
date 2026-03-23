namespace ProRental.Domain.Entities;

public partial class Transactionlog
{
    // Logtype is not in the scaffold — stored as string from DB via shadow property
    // We store it here as a business logic field populated after EF load
    private string? _logtype;
    public void SetLogtype(string? value) => _logtype = value;
    public string? GetLogtype()           => _logtype;

    public int GetTransactionlogid()      => _transactionlogid;
    public DateTime? GetCreatedat()       => _createdat;
}