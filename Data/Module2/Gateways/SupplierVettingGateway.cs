namespace ProRental.Data.Module2.Gateways;

using ProRental.Domain.Entities;
using ProRental.Interfaces.Module2;
using ProRental.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

public class SupplierVettingGateway : ISupplierVettingGateway
{
    private readonly AppDbContext context;

    public SupplierVettingGateway(AppDbContext context)
    {
        this.context = context;
    }

    public List<Supplier> GetUnverifiedSuppliers()
    {
        // AsEnumerable() brings records into memory so the public property accessor can be used,
        // since EF uses field-access mode and cannot translate the property in a SQL WHERE clause.
        return context.Suppliers
            .AsEnumerable()
            .Where(s => s.isverified != true)
            .ToList();
    }
}