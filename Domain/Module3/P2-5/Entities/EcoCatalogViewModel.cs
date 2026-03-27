using ProRental.Domain.Entities;

namespace ProRental.Domain.Module3.P2_5.Entities;

public sealed class EcoCatalogViewModel
{
    public string Search { get; init; } = string.Empty;
    public decimal? MaxBudget { get; init; }
    public string SortBy { get; init; } = "carbon_asc";
    public string Tier { get; init; } = string.Empty;
    public string DebugMessage { get; init; } = string.Empty;
    public List<Catalog> Products { get; init; } = [];
    public int EcoProductCount => Products.Count;
    public decimal? LowestCarbonScore => Products.Count > 0 ? Products.Min(product => product.GetCarbonScore()) : null;
    public string TopEcoBadge => Products.Count > 0 ? Products.First().GetEcoBadge() : "-";
}
