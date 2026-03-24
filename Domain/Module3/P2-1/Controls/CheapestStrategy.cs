using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Orders the shared option set from lowest customer cost to highest.
/// by: ernest
/// </summary>
public sealed class CheapestStrategy : IRankingStrategy
{
    public PreferenceType PreferenceType => PreferenceType.CHEAP;

    public IReadOnlyList<ShippingOptionSummary> Rank(IEnumerable<ShippingOptionSummary> options)
    {
        return ShippingOptionRankingRules.RankByCost(options);  
    }
}
