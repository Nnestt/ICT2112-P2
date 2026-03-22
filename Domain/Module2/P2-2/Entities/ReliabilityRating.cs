namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;

public partial class Reliabilityrating
{
    // Public accessors delegate to the scaffolded Pascal-case private properties,
    // NOT directly to the backing fields — avoids EF field conflict validation errors.
    public int ratingid
    {
        get => Ratingid;
        set => Ratingid = value;
    }

    public int? supplierid
    {
        get => Supplierid;
        set => Supplierid = value;
    }

    public decimal? score
    {
        get => Score;
        set => Score = value;
    }

    public string? rationale
    {
        get => Rationale;
        set => Rationale = value;
    }

    public int? calculatedbyuserid
    {
        get => Calculatedbyuserid;
        set => Calculatedbyuserid = value;
    }

    public DateTime? calculatedat
    {
        get => Calculatedat;
        set => Calculatedat = value;
    }

    // Constants for thresholds
    private const double THRESHOLD_HIGH = 80.0;
    private const double THRESHOLD_MEDIUM = 50.0;
    private const double THRESHOLD_ACCEPTABLE = 50.0;

    // Methods for Rich Domain Model
    public RatingBand GetRatingBand()
    {
        if (this.score == null) return RatingBand.UNRATED;

        double scoreValue = (double)this.score;

        if (scoreValue >= THRESHOLD_HIGH)
            return RatingBand.HIGH;
        else if (scoreValue >= THRESHOLD_MEDIUM)
            return RatingBand.MEDIUM;
        else if (scoreValue >= 0)
            return RatingBand.LOW;
        else
            return RatingBand.UNRATED;
    }

    public bool IsAcceptableRating()
    {
        if (this.score == null) return false;
        return (double)this.score >= THRESHOLD_ACCEPTABLE;
    }

    public bool IsHighRating()   => GetRatingBand() == RatingBand.HIGH;
    public bool IsMediumRating() => GetRatingBand() == RatingBand.MEDIUM;
    public bool IsLowRating()    => GetRatingBand() == RatingBand.LOW;

    public bool MeetsScoreThreshold(double threshold)
    {
        if (this.score == null) return false;
        return (double)this.score >= threshold;
    }
}