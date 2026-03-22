namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;

public partial class Supplier
{
    // Enum backing fields — EF maps these directly via AppDbContext.Custom.cs
    private SupplierCategory _category;
    private SupplierCategory category { get => _category; set => _category = value; }
    public void UpdateCategory(SupplierCategory newValue) => _category = newValue;

    private VettingDecision _decision;
    private VettingDecision decision { get => _decision; set => _decision = value; }
    public void UpdateDecision(VettingDecision newValue) => _decision = newValue;

    // Public accessors delegate to scaffolded Pascal-case private properties,
    // NOT directly to backing fields — avoids EF field conflict validation errors.
    public int supplierid
    {
        get => Supplierid;
        set => Supplierid = value;
    }

    public string? name
    {
        get => Name;
        set => Name = value;
    }

    public string? details
    {
        get => Details;
        set => Details = value;
    }

    public bool? isverified
    {
        get => Isverified;
        set => Isverified = value;
    }

    public int? creditperiod
    {
        get => Creditperiod;
        set => Creditperiod = value;
    }

    public double? avgturnaroundtime
    {
        get => Avgturnaroundtime;
        set => Avgturnaroundtime = value;
    }

    // suppliercategory wraps the EF-owned _category field directly
    // (no Pascal wrapper exists in the scaffold for this enum column)
    public SupplierCategory suppliercategory
    {
        get => _category;
        set => _category = value;
    }
}