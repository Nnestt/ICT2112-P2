// PATH: Data\Module2\Gateways\SupplierMapper.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        SetProperties(entry, supplier);
        _context.Add(dbEntity);
        _context.SaveChanges();
    }

    public void updateSupplier(Supplier supplier)
    {
        // Detach any already-tracked instance with this key to avoid conflicts
        var tracked = _context.ChangeTracker.Entries()
            .FirstOrDefault(e =>
                e.Metadata.ClrType == typeof(ProRental.Domain.Entities.Supplier) &&
                (int)(e.Property("Supplierid").CurrentValue ?? 0) == supplier.SupplierID);

        if (tracked != null)
            tracked.State = EntityState.Detached;

        var dbEntity = _context.Suppliers
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplier.SupplierID);

        if (dbEntity is null)
        {
            insertSupplier(supplier);
            return;
        }

        var entry = _context.Entry(dbEntity);
        SetProperties(entry, supplier);
        _context.SaveChanges();
    }

    public bool deleteSupplier(int supplierID)
    {
        var dbEntity = _context.Suppliers
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplierID);

        if (dbEntity is null) return false;

        _context.Suppliers.Remove(dbEntity);
        _context.SaveChanges();
        return true;
    }

    public Supplier findSupplierById(int supplierID)
    {
        var dbEntity = _context.Suppliers
            .AsNoTracking()
            .SingleOrDefault(s => EF.Property<int>(s, "Supplierid") == supplierID);

        if (dbEntity is null) return null!;
        return MapFromDb(_context.Entry(dbEntity));
    }

    public List<Supplier> findAll()
    {
        return _context.Suppliers
            .AsNoTracking()
            .ToList()
            .Select(e => MapFromDb(_context.Entry(e)))
            .ToList();
    }

    // ── helpers ────────────────────────────────────────────────────────────

    private static void SetProperties(EntityEntry entry, Supplier supplier)
    {
        entry.Property("Supplierid").CurrentValue        = supplier.SupplierID;
        entry.Property("Name").CurrentValue              = supplier.Name;
        entry.Property("Details").CurrentValue           = supplier.Details;
        entry.Property("Creditperiod").CurrentValue      = supplier.CreditPeriod;
        entry.Property("Avgturnaroundtime").CurrentValue = (double)supplier.AvgTurnaroundTime;
        entry.Property("Isverified").CurrentValue        = supplier.IsVerified;
        entry.Property("Suppliercategory").CurrentValue  = supplier.SupplierCategory;
        // DB uses vetting_result_enum, domain uses VettingDecision — same values, convert by name
        entry.Property("Vettingresult").CurrentValue     = Enum.Parse<VettingResult>(supplier.VettingResult.ToString());
    }

    private static Supplier MapFromDb(EntityEntry entry)
    {
        var vettingResultRaw = entry.Property("Vettingresult").CurrentValue;
        var vettingDecision = vettingResultRaw is null
            ? VettingDecision.PENDING
            : Enum.Parse<VettingDecision>(vettingResultRaw.ToString()!);

        var categoryRaw = entry.Property("Suppliercategory").CurrentValue;
        var category = categoryRaw is null
            ? SupplierCategory.NEWUNTESTED
            : (SupplierCategory)categoryRaw;

        return new Supplier
        {
            SupplierID        = (int)(entry.Property("Supplierid").CurrentValue ?? 0),
            Name              = (string)(entry.Property("Name").CurrentValue ?? string.Empty),
            Details           = (string)(entry.Property("Details").CurrentValue ?? string.Empty),
            CreditPeriod      = (int)(entry.Property("Creditperiod").CurrentValue ?? 0),
            AvgTurnaroundTime = (float)(double)(entry.Property("Avgturnaroundtime").CurrentValue ?? 0.0d),
            IsVerified        = (bool)(entry.Property("Isverified").CurrentValue ?? false),
            SupplierCategory  = category,
            VettingResult     = vettingDecision,
        };
    }

    public Supplier getVerifiedSupplierById(int supplierID) => findSupplierById(supplierID);
    public List<Supplier> getVerifiedSuppliers() => findAll().Where(s => s.IsVerified).ToList();
}