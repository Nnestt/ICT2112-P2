namespace ProRental.Interfaces.Module2;

using System.Collections.Generic;

public interface IVerifiedSupplierRegistry
{
    List<string> GetVerifiedSuppliers();
}