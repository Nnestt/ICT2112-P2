// PATH: Controllers\Module2\SupplierRegistryPageController.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Controls;
using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Interfaces.Module2;

namespace ProRental.Controllers.Module2;

public class SupplierRegistryPageController : Controller
{
    private readonly ISupplier _supplier;
    private readonly ISupplierVettingGateway _supplierVettingGateway;
    private readonly IVerifiedSupplierRegistry _verifiedSupplierRegistry;
    private readonly SupplierCategoryChangeLogControl _categoryChangeLogControl;
    // Injected directly so getLatestVettingNote() uses VettingControl's real
    // implementation (which queries the DB), not SupplierControl's stub.
    private readonly VettingControl _vettingControl;
    private string _currentPage = string.Empty;
    private Dictionary<string, string> _requestParams = new();

    public SupplierRegistryPageController(
        ISupplier supplier,
        ISupplierVettingGateway supplierVettingGateway,
        IVerifiedSupplierRegistry verifiedSupplierRegistry,
        SupplierCategoryChangeLogControl categoryChangeLogControl,
        VettingControl vettingControl)
    {
        _supplier = supplier;
        _supplierVettingGateway = supplierVettingGateway;
        _verifiedSupplierRegistry = verifiedSupplierRegistry;
        _categoryChangeLogControl = categoryChangeLogControl;
        _vettingControl = vettingControl;
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
        return View($"~/Views/Module2/{viewName}.cshtml", model);
    }

    public void updateModel(string action, object data) { }

    public object getModelData(string query) => new { query };

    [HttpGet]
    public IActionResult Index()
    {
        var requestParams = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
        handleRequest(requestParams);
        var filter = _requestParams.TryGetValue("filter", out var f) ? f : "all";
        List<Supplier> suppliers = filter switch
        {
            "vetted"     => _verifiedSupplierRegistry.getVettedSuppliers(),
            "unverified" => _supplierVettingGateway.getUnverifiedSuppliers(),
            _            => supplierControl.getAllSuppliers()
        };

        // Build a lookup of supplierID → latest vetting note so the view
        // can show the rejection/amendment reason to Trusted Supplier Registry staff.
        // Uses _vettingControl directly — it has the real DB-backed implementation.
        var vettingNotes = suppliers
            .Where(s => s.VettingResult != ProRental.Domain.Enums.VettingDecision.PENDING)
            .ToDictionary(
                s => s.SupplierID,
                s => _vettingControl.getLatestVettingNote(s.SupplierID) ?? string.Empty);
        ViewData["VettingNotes"] = vettingNotes;

        return renderView("supplierList", suppliers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return renderView("supplierForm", new Supplier());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
        // Load changelogs and pass to view via ViewData
        ViewData["ChangeLogs"] = _categoryChangeLogControl.getLogsBySupplier(id);
        return renderView("supplierForm", supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, string newDetails)
    {
        supplierControl.editSupplier(id, newDetails);
        TempData["Success"] = "Supplier updated.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        supplierControl.deleteSupplier(id);
        TempData["Success"] = "Supplier deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Categorize(int id, SupplierCategory newCategory, string reason)
    {
        var supplier = supplierControl.getSupplierById(id);
        var previousCategory = supplier.SupplierCategory;
        supplierControl.categorizeSupplier(id, newCategory);
        _categoryChangeLogControl.createLog(id, previousCategory, newCategory, reason);
        TempData["Success"] = "Supplier re-categorised.";
        return RedirectToAction(nameof(Edit), new { id });
    }
}