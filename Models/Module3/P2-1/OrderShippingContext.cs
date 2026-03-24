namespace ProRental.Models.Module3.P2_1;

/// <summary>
/// Stable checkout input snapshot passed from the order side into Feature 1 option generation.
/// by: ernest
/// </summary>
public sealed record OrderShippingContext(
    int OrderId,
    int CustomerId,
    int CheckoutId,
    string DestinationAddress,
    double WeightKg,
    int Quantity);
