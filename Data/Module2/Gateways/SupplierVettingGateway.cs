namespace ProRental.Data.Module2.Gateways;

using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Interfaces.Module2;
using ProRental.Data.Module2.Interfaces;
using System.Collections.Generic;
using System.Linq;

public class SupplierVettingGateway : ISupplierVettingGateway
{
    private readonly ISupplierMapper supplierMapper;

    public SupplierVettingGateway(ISupplierMapper supplierMapper)
    {
        this.supplierMapper = supplierMapper;
    }

    public List<Supplier> getUnverifiedSuppliers()
    {
        return supplierMapper.findAll()
            .Where(s => !s.IsVerified)
            .ToList();
    }
}