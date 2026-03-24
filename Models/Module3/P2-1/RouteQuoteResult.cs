namespace ProRental.Models.Module3.P2_1;

/// <summary>
/// Final quote payload returned after Feature 2 prices and carbonizes a selected route.
/// by: ernest
/// </summary>
public sealed record RouteQuoteResult(
    decimal Cost,
    double CarbonFootprintKg);
