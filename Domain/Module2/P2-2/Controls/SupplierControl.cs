using System.Collections.Generic;
using System.Linq;
using ProRental.Data.Module2.Interfaces;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Domain.Module2.P2_2.Factories;
using ProRental.Interfaces.Module2;

namespace ProRental.Domain.Module2.P2_2.Controls;

public class SupplierControl : ISupplier, IVerifiedSupplierRegistry, ISupplierVettingGateway
{
    private readonly ISupplierMapper _supplierMapper;
    private readonly ICategoryChangeLogMapper _categoryChangeLogMapper;
    private readonly SupplierRegistryFactory _factory;

    public SupplierControl(ISupplierMapper supplierMapper, ICategoryChangeLogMapper categoryChangeLogMapper, SupplierRegistryFactory factory)
    {
        _supplierMapper = supplierMapper;
        _categoryChangeLogMapper = categoryChangeLogMapper;
        _factory = factory;
    }

    public Supplier createSupplier(string name, string details, int creditPeriod, float avgTurnaroundTime)
    {
        var entity = _factory.createSupplierRegistryEntity("Supplier") as Supplier;
        if (entity is null)
            return null!;

        entity.Name = name;
        entity.Details = details;
        entity.CreditPeriod = creditPeriod;
        entity.AvgTurnaroundTime = avgTurnaroundTime;
        entity.assignInitialCategory();
        entity.verify(VettingDecision.PENDING);

        _supplierMapper.insertSupplier(entity);
        return entity;
    }

    public void categorizeSupplier(int supplierID)
    {
        categorizeSupplier(supplierID, SupplierCategory.NEWUNTESTED);
    }

    public void categorizeSupplier(int supplierID, SupplierCategory newCategory)
    {
        var supplier = _supplierMapper.findSupplierById(supplierID);
        if (supplier is null)
            return;

        supplier.updateCategory(newCategory);
        supplier.resetVerificationOnRecategorise();
        _supplierMapper.updateSupplier(supplier);
    }

    public void updateSupplierStatus(int supplierID, VettingDecision result)
    {
        var supplier = _supplierMapper.findSupplierById(supplierID);
        if (supplier is null)
            return;

        supplier.verify(result);
        _supplierMapper.updateSupplier(supplier);
    }

    public List<Supplier> getAllSuppliers()
    {
        return _supplierMapper.findAll();
    }

    public Supplier getSupplierById(int supplierID)
    {
        return _supplierMapper.findSupplierById(supplierID);
    }

    public Supplier editSupplier(int supplierID, string newDetails)
    {
        var supplier = _supplierMapper.findSupplierById(supplierID);
        if (supplier is null)
            return null!;

        supplier.Details = newDetails;
        _supplierMapper.updateSupplier(supplier);
        return supplier;
    }

    public bool deleteSupplier(int supplierID)
    {
        return _supplierMapper.deleteSupplier(supplierID);
    }

    public Supplier getVerifiedSupplierById(int supplierID)
    {
        var supplier = _supplierMapper.findSupplierById(supplierID);
        if (supplier is null || !supplier.IsVerified)
            return null!;

        return supplier;
    }

    public List<Supplier> getVerifiedSuppliers()
    {
        return _supplierMapper.findAll().Where(s => s.IsVerified).ToList();
    }

    public List<Supplier> getVettedSuppliers()
    {
        // TODO: IVerifiedSupplierRegistry is implemented by a teammate.
        // Temporarily returning verified suppliers as a stub until integrated.
        // REMOVE this stub implementation once teammate's class is wired in.
        return getVerifiedSuppliers();
    }

    /// <summary>
    /// Stub — SupplierControl has no access to vetting records.
    /// VettingControl, which also implements IVerifiedSupplierRegistry, provides the real implementation.
    /// </summary>
    public string? getLatestVettingNote(int supplierID) => null;

    public List<Supplier> getUnverifiedSuppliers()
    {
        return _supplierMapper.findAll().Where(s => !s.IsVerified).ToList();
    }
}