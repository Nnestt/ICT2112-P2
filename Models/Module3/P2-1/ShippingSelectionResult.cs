using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

public sealed record ShippingSelectionResult(
    int OrderId,
    int OptionId,
    PreferenceType PreferenceType,
    decimal Cost,
    double CarbonFootprintKg,
    int DeliveryDays,
    string TransportModeLabel);
