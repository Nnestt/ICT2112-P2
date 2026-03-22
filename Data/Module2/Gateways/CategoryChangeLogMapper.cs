// PATH: Data\Module2\Gateways\CategoryChangeLogMapper.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProRental.Data.Module2.Interfaces;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Entities;

namespace ProRental.Data.Module2.Gateways;

public class CategoryChangeLogMapper : ICategoryChangeLogMapper
{
    private readonly AppDbContext _context;

    public CategoryChangeLogMapper(AppDbContext context)
    {
        _context = context;
    }

    public void insertCategoryChangeLog(SupplierCategoryChangeLog log)
    {
        var dbEntity = Activator.CreateInstance(typeof(ProRental.Domain.Entities.Suppliercategorychangelog))!;
        var entry = _context.Entry(dbEntity);
        entry.Property("Supplierid").CurrentValue    = log.SupplierID;
        entry.Property("Previouscategory").CurrentValue = log.PreviousCategory;
        entry.Property("Newcategory").CurrentValue   = log.NewCategory;
        entry.Property("Changereason").CurrentValue  = log.ChangedReason;
        entry.Property("Changedat").CurrentValue     = log.ChangedAt;

        _context.Add(dbEntity);
        _context.SaveChanges();

        log.LogID = (int)(_context.Entry(dbEntity).Property("Logid").CurrentValue ?? 0);
    }

    public void updateCategoryChangeLog(SupplierCategoryChangeLog log)
    {
        var dbEntity = _context.Suppliercategorychangelogs
            .SingleOrDefault(l => EF.Property<int>(l, "Logid") == log.LogID);

        if (dbEntity is null)
        {
            insertCategoryChangeLog(log);
            return;
        }

        var entry = _context.Entry(dbEntity);
        entry.Property("Supplierid").CurrentValue       = log.SupplierID;
        entry.Property("Previouscategory").CurrentValue = log.PreviousCategory;
        entry.Property("Newcategory").CurrentValue      = log.NewCategory;
        entry.Property("Changereason").CurrentValue     = log.ChangedReason;
        entry.Property("Changedat").CurrentValue        = log.ChangedAt;

        _context.SaveChanges();
    }

    public bool deleteCategoryChangeLog(int logID)
    {
        var dbEntity = _context.Suppliercategorychangelogs
            .SingleOrDefault(l => EF.Property<int>(l, "Logid") == logID);

        if (dbEntity is null)
            return false;

        _context.Suppliercategorychangelogs.Remove(dbEntity);
        _context.SaveChanges();
        return true;
    }

    public SupplierCategoryChangeLog findCategoryChangeLogById(int logID)
    {
        var dbEntity = _context.Suppliercategorychangelogs
            .AsNoTracking()
            .SingleOrDefault(l => EF.Property<int>(l, "Logid") == logID);

        if (dbEntity is null)
            return null!;

        return MapFromDb(_context.Entry(dbEntity));
    }

    public List<SupplierCategoryChangeLog> findLogsBySupplier(int supplierID)
    {
        var dbEntities = _context.Suppliercategorychangelogs
            .AsNoTracking()
            .Where(l => EF.Property<int?>(l, "Supplierid") == supplierID)
            .ToList();

        return dbEntities.Select(e => MapFromDb(_context.Entry(e))).ToList();
    }

    private static SupplierCategoryChangeLog MapFromDb(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        return new SupplierCategoryChangeLog
        {
            LogID            = (int)(entry.Property("Logid").CurrentValue ?? 0),
            SupplierID       = (int)(entry.Property("Supplierid").CurrentValue ?? 0),
            PreviousCategory = (SupplierCategory)(entry.Property("Previouscategory").CurrentValue ?? SupplierCategory.NEWUNTESTED),
            NewCategory      = (SupplierCategory)(entry.Property("Newcategory").CurrentValue ?? SupplierCategory.NEWUNTESTED),
            ChangedReason    = (string)(entry.Property("Changereason").CurrentValue ?? string.Empty),
            ChangedAt        = (DateTime)(entry.Property("Changedat").CurrentValue ?? DateTime.UtcNow),
        };
    }
}