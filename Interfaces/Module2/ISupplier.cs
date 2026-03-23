using System.Collections.Generic;
using ProRental.Domain.Module2.P2_2.Entities;

namespace ProRental.Interfaces.Module2;

public interface ISupplier
{
    Supplier getVerifiedSupplierById(int supplierID);
    List<Supplier> getVerifiedSuppliers();
}

