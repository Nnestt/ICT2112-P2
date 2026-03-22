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
        // Validate input
        if (!ValidateVettingInput(supplierID, vettedByUserID, decision))
        {
            throw new ArgumentException("Invalid vetting input");
        }

        // Create vetting record
        var record = new Vettingrecord
        {
            supplierid = supplierID,
            vettedbyuserid = vettedByUserID,
            decision_public = decision,
            notes = notes,
            vettedat = vettedAt
        };

        // Persist using mapper
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