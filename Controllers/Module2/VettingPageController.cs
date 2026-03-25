namespace ProRental.Controllers.Module2;

using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Module2.P2_2.Controls;
using ProRental.Interfaces.Module2;
using ProRental.Domain.Module2.P2_2.Strategies;
using ProRental.Domain.Enums;
using ProRental.Data.Module2.Interfaces;
using System;

[Route("module2/[controller]/[action]")]
[StaffAuth]
public class VettingPageController : Controller
{
    private readonly VettingControl vettingControl;
    private readonly SupplierScoringControl scoringControl;
    private readonly ISupplierVettingGateway supplierGateway;
    private readonly ISupplierMapper supplierMapper;

    public VettingPageController(
        VettingControl vettingControl,
        SupplierScoringControl scoringControl,
        ISupplierVettingGateway supplierGateway,
        ISupplierMapper supplierMapper)
    {
        this.vettingControl = vettingControl;
        this.scoringControl = scoringControl;
        this.supplierGateway = supplierGateway;
        this.supplierMapper = supplierMapper;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var suppliers = supplierGateway.getUnverifiedSuppliers();
        return View("~/Views/Module2/DashboardView.cshtml", suppliers);
    }

    [HttpGet]
    public IActionResult VetSupplier(int supplierID)
    {
        ViewBag.SupplierID = supplierID;
        return View("~/Views/Module2/VettingFormView.cshtml");
    }

    [HttpPost]
    public IActionResult SubmitVetting(int supplierID, string notes, int userID, string decision)
    {
        try
        {
            // Parse decision from form
            var vettingDecision = Enum.Parse<VettingDecision>(decision, ignoreCase: true);

            // Record vetting decision
            var record = vettingControl.RecordVetting(
                supplierID, userID, vettingDecision, notes, DateTime.UtcNow);

            // Update supplier verified status based on decision
            var supplier = supplierMapper.findSupplierById(supplierID);
            if (supplier != null)
            {
                supplier.verify(vettingDecision);
                supplierMapper.updateSupplier(supplier);
            }

            ViewBag.VettingRecord = record;
            ViewBag.ReliabilityRating = null;

            return View("~/Views/Module2/VettingFormView.cshtml");
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View("~/Views/Module2/VettingFormView.cshtml");
        }
    }

    [HttpGet]
    public IActionResult GetVettingHistory(int supplierID)
    {
        var history = vettingControl.GetVettingHistory(supplierID);
        return View("~/Views/Module2/VettingHistoryView.cshtml", history);
    }

    [HttpPost]
    public IActionResult EditVettingNotes(int vettingID, string notes, int userID)
    {
        var success = vettingControl.UpdateVettingNotes(vettingID, notes);

        ViewBag.Success = success;
        ViewBag.VettingID = vettingID;

        return View("~/Views/Module2/VettingNotesView.cshtml");
    }

    private IScoringStrategy SelectStrategyForSupplier(int supplierID)
    {
        return new WeightedScoringStrategy();
    }
}