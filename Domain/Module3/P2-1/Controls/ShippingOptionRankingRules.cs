using ProRental.Models.Module3.P2_1;

namespace ProRental.Domain.Controls;

/// <summary>
/// Centralizes deterministic ranking rules shared by the concrete strategies so the
/// compare flow and tests see stable ordering for identical values.
/// by: ernest
/// </summary>
internal static class ShippingOptionRankingRules
{
    public static IReadOnlyList<ShippingOptionSummary> RankBySpeed(IEnumerable<ShippingOptionSummary> options)
    {
        return (options ?? [])
            .Where(option => option is not null)
            .OrderBy(option => option.DeliveryDays)
            .ThenBy(option => option.Cost)
            .ThenBy(option => option.CarbonFootprintKg)
            .ThenBy(option => option.DisplayName, StringComparer.Ordinal)
            .ThenBy(option => option.OptionId)
            .ToArray();
    }

    public static IReadOnlyList<ShippingOptionSummary> RankByCost(IEnumerable<ShippingOptionSummary> options)
    {
        return (options ?? [])
            .Where(option => option is not null)
            .OrderBy(option => option.Cost)
            .ThenBy(option => option.DeliveryDays)
            .ThenBy(option => option.CarbonFootprintKg)
            .ThenBy(option => option.DisplayName, StringComparer.Ordinal)
            .ThenBy(option => option.OptionId)
            .ToArray();
    }

    public static IReadOnlyList<ShippingOptionSummary> RankByCarbon(IEnumerable<ShippingOptionSummary> options)
    {
        return (options ?? [])
            .Where(option => option is not null)
            .OrderBy(option => option.CarbonFootprintKg)
            .ThenBy(option => option.DeliveryDays)
            .ThenBy(option => option.Cost)
            .ThenBy(option => option.DisplayName, StringComparer.Ordinal)
            .ThenBy(option => option.OptionId)
            .ToArray();
    }
}
