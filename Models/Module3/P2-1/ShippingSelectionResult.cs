using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

/// <summary>
/// Confirmation payload returned after the customer selects one shipping option.
/// by: ernest
/// </summary>
public sealed record ShippingSelectionResult(
    int OrderId,
    int OptionId,
    PreferenceType PreferenceType,
    decimal Cost,
    double CarbonFootprintKg,
    int DeliveryDays,
    string TransportModeLabel);
