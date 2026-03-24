using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Orders the shared option set from shortest delivery window to longest.
/// by: ernest
/// </summary>
public sealed class FastestStrategy : IRankingStrategy
{
    public PreferenceType PreferenceType => PreferenceType.FAST;

    public IReadOnlyList<ShippingOptionSummary> Rank(IEnumerable<ShippingOptionSummary> options)
    {
        return ShippingOptionRankingRules.RankBySpeed(options);  
    }
}
