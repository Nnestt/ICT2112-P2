using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class PricingRule
{
    private int _ruleId;
    private int RuleId { get => _ruleId; set => _ruleId = value; }

    private decimal? _baseRatePerKm;
    private decimal? BaseRatePerKm { get => _baseRatePerKm; set => _baseRatePerKm = value; }

    private bool? _isActive;
    private bool? IsActive { get => _isActive; set => _isActive = value; }

    private decimal? _carbonSurcharge;
    private decimal? CarbonSurcharge { get => _carbonSurcharge; set => _carbonSurcharge = value; }
}
