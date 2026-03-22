using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

public sealed record ShippingOptionSummary(
    int OptionId,
    int OrderId,
    PreferenceType PreferenceType,
    string DisplayName,
    decimal Cost,
    double CarbonFootprintKg,
    int DeliveryDays,
    int? RouteId,
    TransportMode? TransportMode,
    string TransportModeLabel);
