using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class ShippingOption
{
    public int GetOptionId() => _optionId;
    public string? GetDisplayName() => _displayName;
    public decimal? GetCost() => _cost;
    public double? GetCarbonFootprintKg() => _carbonfootprintkg;
    public int? GetDeliveryDays() => _deliveryDays;
    public int? GetOrderId() => _orderId;
    public int? GetRouteId() => _routeId;

    public void SetDisplayName(string displayName) => _displayName = displayName;
    public void SetCost(decimal cost) => _cost = cost;
    public void SetCarbonFootprintKg(double carbonFootprintKg) => _carbonfootprintkg = carbonFootprintKg;
    public void SetDeliveryDays(int deliveryDays) => _deliveryDays = deliveryDays;
    public void SetOrderId(int orderId) => _orderId = orderId;
    public void SetRouteId(int routeId) => _routeId = routeId;

    private PreferenceType? _preferenceType;
    private PreferenceType? PreferenceType { get => _preferenceType; set => _preferenceType = value; }
    public PreferenceType? GetPreferenceType() => _preferenceType;
    public void UpdatePreferenceType(PreferenceType newValue) => _preferenceType = newValue;

    private TransportMode? _transportMode;
    private TransportMode? TransportMode { get => _transportMode; set => _transportMode = value; }
    public TransportMode? GetTransportMode() => _transportMode;
    public void UpdateTransportMode(TransportMode newValue) => _transportMode = newValue;
}
