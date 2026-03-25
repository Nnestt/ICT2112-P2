namespace ProRental.Data.Module2.Interfaces;

using ProRental.Domain.Entities;

public interface IReliabilityRatingMapper
{
    Reliabilityrating Insert(Reliabilityrating rating);
    bool Update(Reliabilityrating rating);
    Reliabilityrating FindById(int ratingID);
    Reliabilityrating FindBySupplierID(int supplierID);
}