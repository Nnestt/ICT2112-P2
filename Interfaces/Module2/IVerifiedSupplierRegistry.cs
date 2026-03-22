using System.Collections.Generic;
using ProRental.Domain.Module2.P2_2.Entities;

namespace ProRental.Interfaces.Module2;

interface IVerifiedSupplierRegistry
{
    List<Supplier> getVettedSuppliers();
}

