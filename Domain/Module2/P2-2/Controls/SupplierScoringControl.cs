namespace ProRental.Domain.Module2.P2_2.Controls;

using ProRental.Domain.Entities;
using ProRental.Domain.Module2.P2_2.Strategies;
using ProRental.Data.Module2.Interfaces;
// using ProRental.Interfaces.Module2; // TODO: uncomment when IAnalyticsData is implemented
using System;
using System.Collections.Generic;
using System.Linq;

public class SupplierScoringControl
{
    private IScoringStrategy scoringStrategy;
    // private readonly IAnalyticsData analyticsData; // TODO: uncomment when IAnalyticsData is implemented
    private readonly IReliabilityRatingMapper reliabilityRatingMapper;

    public SupplierScoringControl(
        // IAnalyticsData analyticsData, // TODO: uncomment when IAnalyticsData is implemented
        IReliabilityRatingMapper reliabilityRatingMapper)
    {
        // this.analyticsData = analyticsData; // TODO: uncomment when IAnalyticsData is implemented
        this.reliabilityRatingMapper = reliabilityRatingMapper;
        
        // Default strategy
        this.scoringStrategy = new WeightedScoringStrategy();
    }

    public void SetScoringStrategy(IScoringStrategy strategy)
    {
        this.scoringStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public Reliabilityrating CalculateReliabilityScore(int supplierID, int userID)
    {
        // TODO: Restore analytics lookup when IAnalyticsData is implemented
        // var analytics = analyticsData.GetAnalyticsBySupplier(supplierID.ToString());
        // if (analytics == null || analytics.Count == 0)
        //     throw new InvalidOperationException($"No analytics data found for supplier {supplierID}");
        // double reliability = ExtractReliability(analytics);
        // double turnoverRate = ExtractTurnoverRate(analytics);

        double reliability = 75.0;  // stub
        double turnoverRate = 0.2;  // stub

        // Delegate calculation to strategy
        double score = scoringStrategy.Calculate(reliability, turnoverRate);

        // Validate score
        if (!ValidateScore(score))
        {
            throw new InvalidOperationException($"Invalid score calculated: {score}");
        }

        // Create reliability rating
        var rating = new Reliabilityrating
        {
            supplierid = supplierID,
            score = (decimal)score,
            rationale = GenerateRationale(reliability, turnoverRate, score),
            calculatedbyuserid = userID,
            calculatedat = DateTime.UtcNow
        };

        // Persist rating
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

    // Private helper methods
    private double ExtractReliability(List<Analytic> analytics)
    {
        // TODO: Implement based on your Analytics entity structure
        // This is a placeholder - adjust based on actual analytics data
        return 75.0; // Example value
    }

    private double ExtractTurnoverRate(List<Analytic> analytics)
    {
        // TODO: Implement based on your Analytics entity structure
        // This is a placeholder - adjust based on actual analytics data
        return 0.2; // Example value (20% turnover)
    }

    private string GenerateRationale(double reliability, double turnoverRate, double score)
    {
        return $"Calculated from reliability: {reliability:F2}, turnover rate: {turnoverRate:F2}. " +
               $"Final score: {score:F2}";
    }
}