using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

/// <summary>
/// Lightweight preference card shown before any routing or carbon calculation occurs.
/// by: ernest
/// </summary>
public sealed record ShippingPreferenceCard(
    int OrderId,
    PreferenceType PreferenceType,
    string DisplayName,
    string Description,
    string AllowedModesLabel);
