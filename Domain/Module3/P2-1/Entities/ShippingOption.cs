using ProRental.Domain.Enums;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Entities;

/// <summary>
/// Feature 1 extension for the EF-backed ShippingOption entity. It exposes grouped
/// use-case methods for option generation, checkout display, and customer selection.
/// by: ernest
/// </summary>
public partial class ShippingOption
{
    private PreferenceType? _preferenceType;
    private PreferenceType? PreferenceType { get => _preferenceType; set => _preferenceType = value; }
    private TransportMode? _transportMode;
    private TransportMode? TransportMode { get => _transportMode; set => _transportMode = value; }

    public void ConfigureGeneratedOption(
        int orderId,
        int? routeId,
        PreferenceType preferenceType,
        string displayName,
        decimal cost,
        double carbonFootprintKg,
        int deliveryDays,
        TransportMode transportMode)
    {
        // Feature 1 applies the generated checkout snapshot in one operation so callers
        // do not need field-by-field mutation against the EF entity.
        _orderId = orderId;
        _routeId = routeId;
        _displayName = displayName;
        _cost = cost;
        _carbonfootprintkg = carbonFootprintKg;
        _deliveryDays = deliveryDays;
        _preferenceType = preferenceType;
        _transportMode = transportMode;
    }

    public bool BelongsToOrder(int orderId)
    {
        return _orderId.HasValue && _orderId.Value == orderId;
    }

    public ShippingOptionSummary GetSummary()
    {
        var preferenceType = _preferenceType
            ?? throw new InvalidOperationException($"Shipping option '{_optionId}' is missing its preference type.");

        // The summary is the stable read model shared with the controller, views, and tests.
        return new ShippingOptionSummary(
            _optionId,
            _orderId ?? 0,
            preferenceType,
            _displayName ?? string.Empty,
            _cost ?? 0m,
            _carbonfootprintkg ?? 0d,
            _deliveryDays ?? 0,
            _routeId,
            _transportMode,
            _transportMode?.ToString() ?? string.Empty);
    }

    public ShippingSelectionResult GetSelectionResult()
    {
        var summary = GetSummary();
        return new ShippingSelectionResult(
            summary.OrderId,
            summary.OptionId,
            summary.PreferenceType,
            summary.Cost,
            summary.CarbonFootprintKg,
            summary.DeliveryDays,
            summary.TransportModeLabel);
    }
}
