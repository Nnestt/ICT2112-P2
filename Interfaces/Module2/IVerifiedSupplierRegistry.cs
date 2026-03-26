using System.Collections.Generic;
using ProRental.Domain.Module2.P2_2.Entities;

namespace ProRental.Interfaces.Module2;

public interface IVerifiedSupplierRegistry
{
    List<Supplier> getVettedSuppliers();

    /// <summary>
    /// Returns the most recent vetting notes for a supplier so the
    /// Trusted Supplier Registry can show the reason for rejection /
    /// amendment request.  Returns null if no vetting record exists.
    /// </summary>
    string? getLatestVettingNote(int supplierID);
}