using Microsoft.EntityFrameworkCore;
using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Data;

namespace ProRental.Data;

/* * =========================================================================
 * ARCHITECTURE & AI ASSISTANT RULES FOR THIS FILE
 * =========================================================================
 * 1. STRICT ENCAPSULATION: All entity properties (Inventoryid, Productid, etc.) are PRIVATE. 
 * Do NOT attempt to use standard LINQ (e.g., i.Inventoryid == id). 
 * You MUST use EF.Property<T>(entity, "PropertyName") for all queries.
 * 2. NO CROSS-AGGREGATE INCLUDES: Do NOT use .Include() for Product, Clearanceitem, Loanitems, or Returnitems. 
 * If a developer needs those, they must use their respective mappers.
 * 3. UTC TIMESTAMPS: Always override the "Updatedat" to DateTime.UtcNow 
 * using _context.Entry() during updates to maintain the TIMESTAMPTZ standard.
 * 4. DISCONNECTED UPDATES: Always use CurrentValues.SetValues() to update entities 
 * to avoid tracking conflicts without writing manual property-by-property mapping.
 * =========================================================================
 */

public class InventoryItemMapper : IInventoryItemMapper
{
    private readonly AppDbContext _context;

    public InventoryItemMapper(AppDbContext context)
    {
        _context = context;
    }

    public Inventoryitem? FindById(int inventoryItemId)
    {
        // Using EF.Property to access the private 'Inventoryid'
        // No .Include() to strictly enforce aggregate boundaries
        return _context.Inventoryitems
            .FirstOrDefault(i => EF.Property<int>(i, "Inventoryid") == inventoryItemId);
    }

    public ICollection<Inventoryitem>? FindAll()
    {
        return _context.Inventoryitems.ToList();
    }

    public ICollection<Inventoryitem>? FindByProductId(int productId)
    {
        // Using EF.Property to query against the private 'Productid'
        return _context.Inventoryitems
            .Where(i => EF.Property<int>(i, "Productid") == productId)
            .ToList();
    }

    public ICollection<Inventoryitem>? FindByStatus(InventoryStatus status)
    {
        // Using EF.Property to query against the private 'Status' enum
        return _context.Inventoryitems
            .Where(i => EF.Property<InventoryStatus>(i, "Status") == status)
            .ToList();
    }

    public void Insert(Inventoryitem item)
    {
        _context.Inventoryitems.Add(item);
        _context.SaveChanges();
    }

    public void Update(Inventoryitem item)
    {
        var existing = _context.Inventoryitems
            .FirstOrDefault(i => EF.Property<int>(i, "Inventoryid") == item.GetInventoryItemId());

        if (existing == null) return;

        _context.Entry(existing).CurrentValues.SetValues(item);
        _context.Entry(existing).Property("Updatedat").CurrentValue = DateTime.UtcNow;

        _context.SaveChanges();
    }

    public void Delete(Inventoryitem item)
    {
        var existing = _context.Inventoryitems
            .FirstOrDefault(i => EF.Property<int>(i, "Inventoryid") == item.GetInventoryItemId());
            
        if (existing != null)
        {
            _context.Inventoryitems.Remove(existing);
            _context.SaveChanges();
        }
    }
}