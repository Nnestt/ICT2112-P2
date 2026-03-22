using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class FastestStrategy : IRankingStrategy
{
    public PreferenceType PreferenceType => PreferenceType.FAST;

    public IReadOnlyList<ShippingOptionSummary> Rank(IEnumerable<ShippingOptionSummary> options)
    {
        return ShippingOptionRankingRules.RankBySpeed(options);  
    }
}
