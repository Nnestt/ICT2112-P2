namespace ProRental.Domain.Module2.P2_2.Controls;

using ProRental.Domain.Entities;
using ProRental.Domain.Enums;
using ProRental.Domain.Module2.P2_2.Strategies;
using ProRental.Data.Module2.Interfaces;
using ProRental.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public class SupplierScoringControl
{
    private IScoringStrategy scoringStrategy;
    private readonly IReliabilityRatingMapper reliabilityRatingMapper;
    private readonly IAnalyticsMapper analyticsMapper;

    public SupplierScoringControl(
        IReliabilityRatingMapper reliabilityRatingMapper,
        IAnalyticsMapper analyticsMapper)
    {
        this.reliabilityRatingMapper = reliabilityRatingMapper;
        this.analyticsMapper = analyticsMapper;

        // Default strategy
        this.scoringStrategy = new WeightedScoringStrategy();
    }

    public void SetScoringStrategy(IScoringStrategy strategy)
    {
        this.scoringStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public Reliabilityrating CalculateReliabilityScore(int supplierID, int userID)
    {
        // Fetch all SUPTREND analytics records for this supplier
        var analytics = analyticsMapper.FindBySupplierAsync(supplierID).GetAwaiter().GetResult().ToList();

        double reliability;
        double turnoverRate;

        if (analytics.Count > 0)
        {
            reliability  = ExtractReliability(analytics);
            turnoverRate = ExtractTurnoverRate(analytics);
        }
        else
        {
            // No analytics yet — fall back to neutral defaults
            reliability  = 50.0;
            turnoverRate = 0.5;
        }

        // Delegate calculation to the chosen strategy
        double score = scoringStrategy.Calculate(reliability, turnoverRate);

        // Validate score
        if (!ValidateScore(score))
            throw new InvalidOperationException($"Invalid score calculated: {score}");

        // Build and persist the reliability rating
        var rating = new Reliabilityrating
        {
            supplierid        = supplierID,
            score             = (decimal)score,
            rationale         = GenerateRationale(reliability, turnoverRate, score, analytics.Count),
            calculatedbyuserid = userID,
            calculatedat      = DateTime.UtcNow
        };

        return reliabilityRatingMapper.Insert(rating);
    }

    public Reliabilityrating GetReliabilityRating(int supplierID)
    {
        return reliabilityRatingMapper.FindBySupplierID(supplierID);
    }

    public bool ValidateScore(double score)
    {
        return score >= 0.0 && score <= 100.0;
    }

    /// <summary>
    /// Recommends a vetting decision based on the reliability rating band:
    /// HIGH (>=80) => APPROVED, MEDIUM (50-79) => PENDING (manual review), LOW (<50) => REJECTED.
    /// Staff can override the recommendation before submitting.
    /// </summary>
    public VettingDecision RecommendDecision(Reliabilityrating rating)
    {
        if (rating == null || rating.score == null)
            return VettingDecision.PENDING;

        if (rating.IsHighRating())
            return VettingDecision.APPROVED;
        else if (rating.IsLowRating())
            return VettingDecision.REJECTED;
        else
            return VettingDecision.PENDING;
    }

    /// <summary>Returns a human-readable name for the active scoring strategy.</summary>
    public string GetStrategyName()
    {
        return scoringStrategy.GetType().Name switch
        {
            "SimpleAverageScoringStrategy"       => "Simple Average",
            "WeightedScoringStrategy"            => "Weighted (70% Reliability / 30% Turnover)",
            "PriorityReliabilityScoringStrategy" => "Priority Reliability (90% Reliability / 10% Turnover)",
            var name                             => name
        };
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Extracts the supplier reliability score from SUPTREND analytics.
    /// refValue on SUPTREND records is the reliability metric (generated in the 80–100 range).
    /// We average across all available records to smooth out single-period noise.
    /// The result is scaled to 0–100.
    /// </summary>
    private double ExtractReliability(List<Analytic> analytics)
    {
        var values = analytics
            .Select(a => (double?)a.GetRefValue())
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

        if (values.Count == 0) return 50.0;

        // refValue is already in 0–100 range for SUPTREND records
        return Math.Clamp(values.Average(), 0.0, 100.0);
    }

    /// <summary>
    /// Derives turnover rate from loan and return counts across SUPTREND analytics.
    /// turnoverRate = total returns / total loans (clamped to [0, 1]).
    /// A high turnover rate (many returns relative to loans) lowers the final score.
    /// Falls back to 0.5 (neutral) when no loan data is available.
    /// </summary>
    private double ExtractTurnoverRate(List<Analytic> analytics)
    {
        int totalLoans   = analytics.Sum(a => a.GetLoanAmt()   ?? 0);
        int totalReturns = analytics.Sum(a => a.GetReturnAmt() ?? 0);

        if (totalLoans <= 0) return 0.5; // neutral fallback

        return Math.Clamp((double)totalReturns / totalLoans, 0.0, 1.0);
    }

    private string GenerateRationale(double reliability, double turnoverRate, double score, int recordCount)
    {
        string dataSource = recordCount > 0
            ? $"Based on {recordCount} analytics record(s)."
            : "No analytics data found — neutral defaults applied.";

        return $"{dataSource} Strategy: {GetStrategyName()}. " +
               $"Reliability: {reliability:F1}, Turnover Rate: {turnoverRate:P0}. " +
               $"Final Score: {score:F1}";
    }
}