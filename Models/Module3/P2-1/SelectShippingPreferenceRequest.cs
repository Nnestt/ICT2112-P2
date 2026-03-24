using ProRental.Domain.Enums;

namespace ProRental.Models.Module3.P2_1;

/// <summary>
/// Customer selection payload for the deferred Feature 1 preference flow.
/// by: ernest
/// </summary>
public sealed record SelectShippingPreferenceRequest(int OrderId, PreferenceType PreferenceType);
