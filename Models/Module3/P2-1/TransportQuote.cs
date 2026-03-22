using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

public sealed record TransportQuote(
    decimal Cost,
    double CarbonFootprintKg,
    int DeliveryDays,
    TransportMode TransportMode,
    string DisplayName,
    string TransportModeLabel);
