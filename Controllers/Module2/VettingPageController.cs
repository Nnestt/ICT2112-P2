namespace ProRental.Controllers.Module2;

using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Module2.P2_2.Controls;
using ProRental.Interfaces.Module2;
using ProRental.Domain.Module2.P2_2.Strategies;
using ProRental.Domain.Enums;
using System;

public class VettingPageController : Controller
{
    private readonly VettingControl vettingControl;
    private readonly SupplierScoringControl scoringControl;
    private readonly ISupplierVettingGateway supplierGateway;

    public VettingPageController(
        VettingControl vettingControl,
        SupplierScoringControl scoringControl,
        ISupplierVettingGateway supplierGateway)
    {
        this.vettingControl = vettingControl;
        this.scoringControl = scoringControl;
        this.supplierGateway = supplierGateway;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var suppliers = supplierGateway.GetUnverifiedSuppliers();
        return View("~/Views/Module2/DashboardView.cshtml", suppliers);
    }

    [HttpPost]
    public IActionResult SubmitVetting(int supplierID, string notes, int userID)
    {
        try
        {
            // Select appropriate scoring strategy based on supplier characteristics
            var strategy = SelectStrategyForSupplier(supplierID);
            scoringControl.SetScoringStrategy(strategy);

            // Calculate reliability score
            var rating = scoringControl.CalculateReliabilityScore(supplierID, userID);

            // Determine vetting decision based on rating
            var decision = rating.IsAcceptableRating() ? 
                VettingDecision.APPROVED : VettingDecision.REJECTED;

            // Record vetting decision
            var record = vettingControl.RecordVetting(
                supplierID, userID, decision, notes, DateTime.UtcNow);

            ViewBag.VettingRecord = record;
            ViewBag.ReliabilityRating = rating;

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

    // Private helper method
    private IScoringStrategy SelectStrategyForSupplier(int supplierID)
    {
        // TODO: Implement business logic to determine strategy
        // For now, default to weighted strategy
        return new WeightedScoringStrategy();
    }
}