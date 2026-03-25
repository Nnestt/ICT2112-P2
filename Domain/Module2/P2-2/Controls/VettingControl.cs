namespace ProRental.Domain.Module2.P2_2.Controls;

using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Data.Module2.Interfaces;
using ProRental.Interfaces.Module2;
using System;
using System.Collections.Generic;

public class VettingControl : IVerifiedSupplierRegistry
{
    private readonly IVettingRecordMapper vettingRecordMapper;

    public VettingControl(IVettingRecordMapper vettingRecordMapper)
    {
        this.vettingRecordMapper = vettingRecordMapper;
    }

    public Vettingrecord RecordVetting(
        int supplierID,
        int vettedByUserID,
        VettingDecision decision,
        string notes,
        DateTime vettedAt)
    {
        if (!ValidateVettingInput(supplierID, vettedByUserID, decision))
        {
            throw new ArgumentException("Invalid vetting input");
        }

        var record = new Vettingrecord
        {
            supplierid = supplierID,
            vettedbyuserid = vettedByUserID,
            decision_public = decision,
            notes = notes,
            vettedat = vettedAt
        };

        return vettingRecordMapper.Insert(record);
    }

    public bool UpdateVettingNotes(int vettingID, string notes)
    {
        var record = vettingRecordMapper.FindById(vettingID);
        if (record == null) return false;

        record.notes = notes;
        return vettingRecordMapper.Update(record);
    }

    public List<Vettingrecord> GetVettingHistory(int supplierID)
    {
        return vettingRecordMapper.FindBySupplierID(supplierID);
    }

    // Implements IVerifiedSupplierRegistry.getVettedSuppliers()
    public List<ProRental.Domain.Module2.P2_2.Entities.Supplier> getVettedSuppliers()
    {
        // Returns an empty list for now — to be implemented when supplier data layer is connected
        return new List<ProRental.Domain.Module2.P2_2.Entities.Supplier>();
    }

    // Implements IVerifiedSupplierRegistry.getLatestVettingNote()
    public string? getLatestVettingNote(int supplierID)
    {
        var history = vettingRecordMapper.FindBySupplierID(supplierID);
        // FindBySupplierID already orders by vettedat descending
        return history.FirstOrDefault()?.notes;
    }

    public List<string> GetVerifiedSuppliers()
    {
        var approvedRecords = vettingRecordMapper.FindAllApproved();
        var supplierIDs = new List<string>();

        foreach (var record in approvedRecords)
        {
            if (record.supplierid.HasValue)
            {
                supplierIDs.Add(record.supplierid.Value.ToString());
            }
        }

        return supplierIDs;
    }

    public bool ValidateVettingInput(int supplierID, int userID, VettingDecision decision)
    {
        if (supplierID <= 0) return false;
        if (userID <= 0) return false;
        if (decision != VettingDecision.APPROVED &&
            decision != VettingDecision.REJECTED &&
            decision != VettingDecision.PENDING)
        {
            return false;
        }
        return true;
    }
}