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
        SupplierCategory = SupplierCategory.NEWUNTESTED;
    }

    public void updateCategory(SupplierCategory newCategory)
    {
        SupplierCategory = newCategory;
    }

    public void verify(VettingDecision result)
    {
        VettingResult = result;
        IsVerified = (result == VettingDecision.APPROVED);
    }

    public SupplierCategory getCategory() => SupplierCategory;

    public string getDetails() => Details;

    string ISupplierRegistryEntity.GetType() => "Supplier";
}