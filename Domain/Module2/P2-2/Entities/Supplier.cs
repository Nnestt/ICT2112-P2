using ProRental.Domain.Enums;
using ProRental.Interfaces.Module2;

namespace ProRental.Domain.Module2.P2_2.Entities;

public class Supplier : ISupplierRegistryEntity
{
    public int SupplierID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int CreditPeriod { get; set; }
    public float AvgTurnaroundTime { get; set; }
    public SupplierCategory SupplierCategory { get; set; }
    public bool IsVerified { get; set; }
    public VettingDecision VettingResult { get; set; }

    public Supplier()
    {
        SupplierID = 0;
        SupplierCategory = SupplierCategory.NEWUNTESTED;
        VettingResult = VettingDecision.PENDING;
        IsVerified = false;
    }

    public void assignInitialCategory()
    {
        if (CreditPeriod > 30)
            SupplierCategory = SupplierCategory.LONGCREDITPERIOD;
        else if (AvgTurnaroundTime < 5)
            SupplierCategory = SupplierCategory.QUICKTURNAROUNDTIME;
        else
            SupplierCategory = SupplierCategory.NEWUNTESTED;
    }

    public void updateCategory(SupplierCategory newCategory)
    {
        SupplierCategory = newCategory;
    }

    public void verify(VettingDecision result)
    {
        VettingResult = result;
        // Mark as verified (i.e. vetting has been actioned) for both APPROVED and REJECTED,
        // so the supplier no longer appears on the unverified dashboard.
        // IsVerified == true means "vetting has been submitted", NOT "supplier is trusted".
        // Use VettingResult to distinguish approved vs rejected.
        IsVerified = (result == VettingDecision.APPROVED || result == VettingDecision.REJECTED);
    }


    public void resetVerificationOnRecategorise()
    {
    VettingResult = VettingDecision.PENDING;
    IsVerified = false;
    }


    public SupplierCategory getCategory() => SupplierCategory;

    public string getDetails() => Details;

    string ISupplierRegistryEntity.GetType() => "Supplier";
}