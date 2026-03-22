namespace ProRental.Interfaces.Module2;

using ProRental.Domain.Entities;
using System.Collections.Generic;

public interface ISupplierVettingGateway
{
    List<Supplier> GetUnverifiedSuppliers();
}