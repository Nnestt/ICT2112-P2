using ProRental.Domain.Enums;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Interfaces.Domain;

public interface IRankingStrategy
{
    PreferenceType PreferenceType { get; }
    IReadOnlyList<ShippingOptionSummary> Rank(IEnumerable<ShippingOptionSummary> options);
}
