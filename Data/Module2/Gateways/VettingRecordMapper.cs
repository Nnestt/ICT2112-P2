namespace ProRental.Data.Module2.Gateways;

using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Data.Module2.Interfaces;
using ProRental.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

public class VettingRecordMapper : IVettingRecordMapper
{
    private readonly AppDbContext context;

    public VettingRecordMapper(AppDbContext context)
    {
        this.context = context;
    }

    public Vettingrecord Insert(Vettingrecord record)
    {
        context.Vettingrecords.Add(record);
        context.SaveChanges();
        return record;
    }

    public bool Update(Vettingrecord record)
    {
        try
        {
            context.Vettingrecords.Update(record);
            context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Vettingrecord FindById(int vettingID)
    {
        return context.Vettingrecords.Find(vettingID)!;
    }

    public List<Vettingrecord> FindBySupplierID(int supplierID)
    {
        return context.Vettingrecords
            .Where(v => v.supplierid == supplierID)
            .OrderByDescending(v => v.vettedat)
            .ToList();
    }

    public List<Vettingrecord> FindAllApproved()
    {
        return context.Vettingrecords
            .Where(v => v.decision_public == VettingDecision.APPROVED)
            .ToList();
    }

    public bool Delete(int vettingID)
    {
        try
        {
            var record = FindById(vettingID);
            if (record == null) return false;

            context.Vettingrecords.Remove(record);
            context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}