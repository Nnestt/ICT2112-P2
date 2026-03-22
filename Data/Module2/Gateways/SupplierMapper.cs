// PATH: Data\Module2\Gateways\SupplierMapper.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module2.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Entities;

namespace ProRental.Data.Module2.Gateways;

public class SupplierMapper : ISupplierMapper
{
    private readonly AppDbContext _context;

    public SupplierMapper(AppDbContext context)
    {
        _context = context;
    }

    public void insertSupplier(Supplier supplier)
    {
        if (supplier.SupplierID == 0)
        {
            int? maxId = _context.Suppliers
                .Select(s => (int?)EF.Property<int>(s, "Supplierid"))
                .Max();
            supplier.SupplierID = (maxId ?? 0) + 1;
        }

        var dbEntity = Activator.CreateInstance(typeof(ProRental.Domain.Entities.Supplier))!;
        var entry = _context.Entry(dbEntity);
        entry.Property("Supplierid").CurrentValue       = supplier.SupplierID;
        entry.Property("Name").CurrentValue             = supplier.Name;
        entry.Property("Details").CurrentValue          = supplier.Details;
        entry.Property("Creditperiod").CurrentValue     = supplier.CreditPeriod;
        entry.Property("Avgturnaroundtime").CurrentValue = (double)supplier.AvgTurnaroundTime;
        entry.Property("Isverified").CurrentValue       = supplier.IsVerified;
        entry.Property("Suppliercategory").CurrentValue = supplier.SupplierCategory;
        entry.Property("Vettingresult").CurrentValue    = supplier.VettingResult;

        _context.Add(dbEntity);
        _context.SaveChanges();
    }

    public void updateSupplier(Supplier supplier)
    {
        var dbEntity = _context.Suppliers
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplier.SupplierID);

        if (dbEntity is null)
        {
            insertSupplier(supplier);
            return;
        }

        var entry = _context.Entry(dbEntity);
        entry.Property("Name").CurrentValue             = supplier.Name;
        entry.Property("Details").CurrentValue          = supplier.Details;
        entry.Property("Creditperiod").CurrentValue     = supplier.CreditPeriod;
        entry.Property("Avgturnaroundtime").CurrentValue = (double)supplier.AvgTurnaroundTime;
        entry.Property("Isverified").CurrentValue       = supplier.IsVerified;
        entry.Property("Suppliercategory").CurrentValue = supplier.SupplierCategory;
        entry.Property("Vettingresult").CurrentValue    = supplier.VettingResult;

        _context.SaveChanges();
    }

    public bool deleteSupplier(int supplierID)
    {
        var dbEntity = _context.Suppliers
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplierID);

        if (dbEntity is null)
            return false;

        _context.Suppliers.Remove(dbEntity);
        _context.SaveChanges();
        return true;
    }

    public Supplier findSupplierById(int supplierID)
    {
        var dbEntity = _context.Suppliers
            .AsNoTracking()
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplierID);

        if (dbEntity is null)
            return null!;

        return MapFromDb(dbEntity);
    }

    public List<Supplier> findAll()
    {
        return _context.Suppliers
            .AsNoTracking()
            .ToList()
            .Select(s => MapFromDb(s))
            .ToList();
    }

    private static Supplier MapFromDb(dynamic row)
    {
        return new Supplier
        {
            SupplierID        = row.Supplierid,
            Name              = row.Name             ?? string.Empty,
            Details           = row.Details          ?? string.Empty,
            CreditPeriod      = row.Creditperiod      ?? 0,
            AvgTurnaroundTime = (float)(row.Avgturnaroundtime ?? 0.0d),
            SupplierCategory  = row.Suppliercategory  ?? SupplierCategory.NEWUNTESTED,
            IsVerified        = row.Isverified        ?? false,
            VettingResult     = row.Vettingresult     ?? VettingDecision.PENDING
        };
    }

    public Supplier getVerifiedSupplierById(int supplierID) => findSupplierById(supplierID);

    public List<Supplier> getVerifiedSuppliers() => findAll().Where(s => s.IsVerified).ToList();
}