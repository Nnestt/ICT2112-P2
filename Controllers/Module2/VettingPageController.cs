namespace ProRental.Controllers.Module2;

using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Module2.P2_2.Controls;
using ProRental.Domain.Module2.P2_2.Entities;
using ProRental.Domain.Module2.P2_2.Strategies;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module2;
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

    /// <summary>
    /// Loads the vetting form. Automatically runs the scoring strategy for the supplier
    /// so the staff member sees the reliability score and recommendation before deciding.
    /// </summary>
    [HttpGet]
    public IActionResult VetSupplier(int supplierID)
    {
        var supplier = supplierMapper.findSupplierById(supplierID);
        if (supplier == null)
        {
            ViewBag.Error = $"Supplier {supplierID} not found.";
            return View("~/Views/Module2/VettingFormView.cshtml");
        }

        try
        {
            // Select strategy based on supplier category
            var strategy = SelectStrategyForSupplier(supplier);
            scoringControl.SetScoringStrategy(strategy);

            // Calculate reliability score (uses stub data until analytics is wired)
            // userID 0 = system-generated score, not tied to a specific staff member
            var rating = scoringControl.CalculateReliabilityScore(supplierID, userID: 0);

            // Derive recommended decision from the score
            var recommendation = scoringControl.RecommendDecision(rating);

            ViewBag.SupplierID = supplierID;
            ViewBag.Supplier = supplier;
            ViewBag.ReliabilityRating = rating;
            ViewBag.Recommendation = recommendation;
            ViewBag.StrategyName = scoringControl.GetStrategyName();
        }
        catch (Exception ex)
        {
            // Scoring failure should not block vetting — staff can still proceed manually
            ViewBag.SupplierID = supplierID;
            ViewBag.Supplier = supplier;
            ViewBag.ScoringError = ex.Message;
        }

        return View("~/Views/Module2/VettingFormView.cshtml");
    }

    /// <summary>
    /// Records the vetting decision submitted by staff.
    /// The staff decision is authoritative — the scoring recommendation is advisory only.
    /// </summary>
    [HttpPost]
    public IActionResult SubmitVetting(int supplierID, string notes, int userID, string decision)
    {
        try
        {
            var vettingDecision = Enum.Parse<VettingDecision>(decision, ignoreCase: true);

            // Record vetting decision
            var record = vettingControl.RecordVetting(
                supplierID, userID, vettingDecision, notes, DateTime.UtcNow);

            // Update supplier verified status
            var supplier = supplierMapper.findSupplierById(supplierID);
            if (supplier != null)
            {
                supplier.verify(vettingDecision);
                supplierMapper.updateSupplier(supplier);
            }

            // Retrieve latest reliability rating to show alongside the vetting result
            var rating = scoringControl.GetReliabilityRating(supplierID);

            ViewBag.VettingRecord = record;
            ViewBag.ReliabilityRating = rating;
            ViewBag.StrategyName = scoringControl.GetStrategyName();

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

    /// <summary>
    /// Selects a scoring strategy based on the supplier's category:
    /// - NEWUNTESTED           => SimpleAverageScoringStrategy  (baseline, equal weighting)
    /// - QUICKTURNAROUNDTIME   => WeightedScoringStrategy        (reliability 70%, turnover 30%)
    /// - LONGCREDITPERIOD      => PriorityReliabilityScoringStrategy (reliability 90%, turnover 10%)
    ///
    /// Rationale: suppliers offering long credit periods carry higher financial risk, so
    /// reliability is weighted most heavily. Quick-turnaround suppliers are weighted in the
    /// middle. New/untested suppliers get a neutral simple average.
    /// </summary>
    private IScoringStrategy SelectStrategyForSupplier(Supplier supplier)
    {
        return supplier.SupplierCategory switch
        {
            SupplierCategory.LONGCREDITPERIOD    => new PriorityReliabilityScoringStrategy(),
            SupplierCategory.QUICKTURNAROUNDTIME => new WeightedScoringStrategy(),
            _                                    => new SimpleAverageScoringStrategy()
        };
    }
}