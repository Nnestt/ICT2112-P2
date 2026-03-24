using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class PricingRule
{
    private TransportMode? _transportMode;
    private TransportMode? TransportMode { get => _transportMode; set => _transportMode = value; }

    private int getRuleId() => _ruleId;

    private TransportMode? getTransportMode() => _transportMode;

    private void setTransportMode(TransportMode? transportMode)
    {
        _transportMode = transportMode;
    }

    private decimal? getBaseRatePerKm() => _baseRatePerKm;

    private void setBaseRatePerKm(decimal? baseRatePerKm)
    {
        _baseRatePerKm = baseRatePerKm;
    }

    private bool getIsActive() => _isActive ?? false;

    private void setIsActive(bool isActive)
    {
        _isActive = isActive;
    }

    private decimal? getCarbonSurcharge() => _carbonSurcharge;

    private void setCarbonSurcharge(decimal? carbonSurcharge)
    {
        _carbonSurcharge = carbonSurcharge;
    }

    public int ReadRuleId() => getRuleId();

    public TransportMode? ReadTransportMode() => getTransportMode();

    public decimal? ReadBaseRatePerKm() => getBaseRatePerKm();

    public bool ReadIsActive() => getIsActive();

    public decimal? ReadCarbonSurcharge() => getCarbonSurcharge();
}
