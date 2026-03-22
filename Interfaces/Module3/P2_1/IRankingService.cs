using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Module3.P2_1;

public interface IRankingService
{
    IReadOnlyList<ShippingOptionSummary> RankBySpeed(IEnumerable<ShippingOptionSummary> options);
    IReadOnlyList<ShippingOptionSummary> RankByCost(IEnumerable<ShippingOptionSummary> options);
    IReadOnlyList<ShippingOptionSummary> RankByCarbon(IEnumerable<ShippingOptionSummary> options);
}
