using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Controls;
using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Interfaces.Module2;

namespace ProRental.Controllers.Module2;

class SupplierRegistryPageController : Controller
{
    private readonly ISupplier _supplier;
    private readonly ISupplierVettingGateway _supplierVettingGateway;
    private readonly IVerifiedSupplierRegistry _verifiedSupplierRegistry;
    private readonly SupplierCategoryChangeLogControl _categoryChangeLogControl;

    private string _currentPage = string.Empty;
    private Dictionary<string, string> _requestParams = new();

    public SupplierRegistryPageController(
        ISupplier supplier,
        ISupplierVettingGateway supplierVettingGateway,
        IVerifiedSupplierRegistry verifiedSupplierRegistry,
        SupplierCategoryChangeLogControl categoryChangeLogControl)
    {
        _supplier = supplier;
        _supplierVettingGateway = supplierVettingGateway;
        _verifiedSupplierRegistry = verifiedSupplierRegistry;
        _categoryChangeLogControl = categoryChangeLogControl;
    }

    private SupplierControl supplierControl
        => _supplier as SupplierControl
           ?? throw new System.InvalidOperationException("ISupplier is not backed by SupplierControl.");

    public void handleRequest(Dictionary<string, string> paramsDict)
    {
        _requestParams = paramsDict;
        _currentPage = _requestParams.TryGetValue("page", out var page) ? page : string.Empty;
    }

    public IActionResult renderView(string viewName, object model)
    {
        return View(viewName, model);
    }

    public void updateModel(string action, object data)
    {
        // Intentionally left as a small extension point for future integration.
    }

    public object getModelData(string query)
    {
        return new { query };
    }

    [HttpGet]
    public IActionResult Index()
    {
        var requestParams = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
        handleRequest(requestParams);

        var filter = _requestParams.TryGetValue("filter", out var f) ? f : "all";

        List<Supplier> suppliers = filter switch
        {
            "vetted" => _verifiedSupplierRegistry.getVettedSuppliers(),
            "unverified" => _supplierVettingGateway.getUnverifiedSuppliers(),
            _ => supplierControl.getAllSuppliers()
        };

        return renderView("supplierList", suppliers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new Supplier();
        return renderView("supplierForm", model);
    }

    [HttpPost]
    public IActionResult Create(string name, string details, int creditPeriod, float avgTurnaroundTime)
    {
        try
        {
            supplierControl.createSupplier(name, details, creditPeriod, avgTurnaroundTime);
            TempData["Success"] = "Supplier created successfully.";
        }
        catch
        {
            TempData["Error"] = "Failed to create supplier.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var supplier = supplierControl.getSupplierById(id);
        return renderView("supplierForm", supplier);
    }

    [HttpPost]
    public IActionResult Edit(int id, string newDetails)
    {
        supplierControl.editSupplier(id, newDetails);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        supplierControl.deleteSupplier(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Verify(int id, VettingDecision result)
    {
        supplierControl.updateSupplierStatus(id, result);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Categorize(int id, SupplierCategory newCategory, string reason)
    {
        var supplier = supplierControl.getSupplierById(id);
        var previousCategory = supplier.SupplierCategory;

        supplierControl.categorizeSupplier(id, newCategory);
        _categoryChangeLogControl.createLog(id, previousCategory, newCategory, reason);

        return RedirectToAction(nameof(Index));
    }
}

