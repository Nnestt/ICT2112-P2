namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;

public partial class Vettingrecord
{
    // Enum backing field — EF maps this directly via AppDbContext.Custom.cs
    private VettingDecision _decision;
    private VettingDecision decision { get => _decision; set => _decision = value; }

    // Public accessor for decision wraps the EF-owned _decision field directly
    // (no Pascal wrapper exists in the scaffold for this enum column)
    public VettingDecision decision_public
    {
        get => _decision;
        set => _decision = value;
    }

    // Public accessors delegate to scaffolded Pascal-case private properties,
    // NOT directly to backing fields — avoids EF field conflict validation errors.
    public int vettingid
    {
        get => Vettingid;
        set => Vettingid = value;
    }

    public int? supplierid
    {
        get => Supplierid;
        set => Supplierid = value;
    }

    public int? vettedbyuserid
    {
        get => Vettedbyuserid;
        set => Vettedbyuserid = value;
    }

    public DateTime? vettedat
    {
        get => Vettedat;
        set => Vettedat = value;
    }

    public string? notes
    {
        get => Notes;
        set => Notes = value;
    }

    // Methods for Rich Domain Model
    public bool IsApproved()  => _decision == VettingDecision.APPROVED;
    public bool IsRejected()  => _decision == VettingDecision.REJECTED;
    public bool CanBeEditedBy(int userID) => vettedbyuserid == userID;

    public bool Validate()
    {
        if (supplierid == null || supplierid <= 0) return false;
        if (vettedbyuserid == null || vettedbyuserid <= 0) return false;
        if (vettedat == null) return false;
        if (string.IsNullOrWhiteSpace(notes)) return false;
        return true;
    }
}