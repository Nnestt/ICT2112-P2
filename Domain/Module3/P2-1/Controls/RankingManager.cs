using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Feature 1 ranking coordinator. It resolves the registered strategy for each
/// preference dimension and keeps the controller independent of concrete algorithms.
/// by: ernest
/// </summary>
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
