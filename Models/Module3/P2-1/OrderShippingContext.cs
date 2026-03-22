namespace ProRental.Models.Module3.P2_1;

public sealed record OrderShippingContext(
    int OrderId,
    int CustomerId,
    int CheckoutId,
    string DestinationAddress,
    double WeightKg,
    int Quantity);
