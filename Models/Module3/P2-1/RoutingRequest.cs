using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

public sealed record RoutingRequest(
    int OrderId,
    string DestinationAddress,
    double WeightKg,
    int Quantity,
    PreferenceType PreferenceType);
