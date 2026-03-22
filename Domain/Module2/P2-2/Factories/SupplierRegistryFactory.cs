using System;
using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Interfaces.Module2;

namespace ProRental.Domain.Module2.P2_2.Factories;

class SupplierRegistryFactory
{
    public ISupplierRegistryEntity createSupplierRegistryEntity(string type)
    {
        return type switch
        {
            "Supplier" => new Supplier(),
            "SupplierCategoryChangeLog" => new SupplierCategoryChangeLog(),
            _ => throw new ArgumentException($"Unknown supplier registry entity type: {type}", nameof(type))
        };
    }
}

