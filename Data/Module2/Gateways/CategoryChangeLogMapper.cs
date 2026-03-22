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
        var scaffoldType = typeof(ProRental.Domain.Entities.Suppliercategorychangelog);
        var dbEntity = Activator.CreateInstance(scaffoldType)!;
 
        var entry = _context.Entry(dbEntity);
        entry.Property("Supplierid").CurrentValue       = log.SupplierID;
        entry.Property("Previouscategory").CurrentValue = log.PreviousCategory;
        entry.Property("Newcategory").CurrentValue      = log.NewCategory;
        entry.Property("Changereason").CurrentValue     = log.ChangedReason;
        entry.Property("Changedat").CurrentValue        = log.ChangedAt;
 
        _context.Add(dbEntity);
        _context.SaveChanges();
 
        log.LogID = (int)_context.Entry(dbEntity).Property("Logid").CurrentValue!;
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
 
        return MapFromDb(dbEntity);
    }
 
    public List<SupplierCategoryChangeLog> findLogsBySupplier(int supplierID)
    {
        var rows = _context.Suppliercategorychangelogs
            .AsNoTracking()
            .Where(l => EF.Property<int?>(l, "Supplierid") == supplierID)
            .ToList();
 
        return rows.Select(MapFromDb).ToList();
    }
 
    private static SupplierCategoryChangeLog MapFromDb(dynamic row)
    {
        return new SupplierCategoryChangeLog
        {
            LogID            = row.Logid,
            SupplierID       = row.Supplierid        ?? 0,
            PreviousCategory = row.Previouscategory  ?? SupplierCategory.NEWUNTESTED,
            NewCategory      = row.Newcategory        ?? SupplierCategory.NEWUNTESTED,
            ChangedReason    = row.Changereason       ?? string.Empty,
            ChangedAt        = row.Changedat          ?? System.DateTime.UtcNow
        };
    }
}