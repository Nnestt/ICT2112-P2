using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class EcoFriendlyStrategy : IRankingStrategy
{
    public PreferenceType PreferenceType => PreferenceType.GREEN;

    public IReadOnlyList<ShippingOptionSummary> Rank(IEnumerable<ShippingOptionSummary> options)
    {
        return ShippingOptionRankingRules.RankByCarbon(options);
    }
}
