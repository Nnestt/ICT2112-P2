namespace ProRental.Data.Module2.Interfaces;

using ProRental.Domain.Entities;
using System.Collections.Generic;

public interface IVettingRecordMapper
{
    Vettingrecord Insert(Vettingrecord record);
    bool Update(Vettingrecord record);
    Vettingrecord FindById(int vettingID);
    List<Vettingrecord> FindBySupplierID(int supplierID);
    List<Vettingrecord> FindAllApproved();
}