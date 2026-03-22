using ProRental.Domain.Enums;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

public sealed class RankingManager : IRankingService
{
    private readonly IReadOnlyDictionary<PreferenceType, IRankingStrategy> _strategies;

    public RankingManager(IEnumerable<IRankingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(strategy => strategy.PreferenceType);
    }

    public IReadOnlyList<ShippingOptionSummary> RankBySpeed(IEnumerable<ShippingOptionSummary> options)
    {
        return GetStrategy(PreferenceType.FAST).Rank(options);
    }

    public IReadOnlyList<ShippingOptionSummary> RankByCost(IEnumerable<ShippingOptionSummary> options)
    {
        return GetStrategy(PreferenceType.CHEAP).Rank(options);
    }

    public IReadOnlyList<ShippingOptionSummary> RankByCarbon(IEnumerable<ShippingOptionSummary> options)
    {
        return GetStrategy(PreferenceType.GREEN).Rank(options);
    }

    private IRankingStrategy GetStrategy(PreferenceType preferenceType)
    {
        if (_strategies.TryGetValue(preferenceType, out var strategy))
        {
            return strategy;
        }

        throw new InvalidOperationException($"No ranking strategy registered for '{preferenceType}'.");
    }
}
