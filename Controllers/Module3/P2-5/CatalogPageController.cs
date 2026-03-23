using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Module3.P2_5.Controls;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Data.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5
{
    [Route("catalog")]
    public class CatalogPageController : Controller
    {
        private readonly CatalogControl _control;

        public CatalogPageController(ICatalogGateway catalogGateway)
        {
            _control = new CatalogControl(catalogGateway);
        }

        [HttpGet("eco")]
        public IActionResult Eco(string? search, decimal? maxBudget, string? sortBy, string? tier)
        {
            try
            {
                var products = _control.GetSortedByCarbon();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var keyword = search.Trim();
                    products = products
                        .Where(product =>
                            product.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            product.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            product.EcoBadge.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            GetEcoTier(product.CarbonScore).Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (maxBudget.HasValue)
                {
                    products = products
                        .Where(product => product.Price <= maxBudget.Value)
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(tier))
                {
                    products = products
                        .Where(product => string.Equals(GetEcoTier(product.CarbonScore), tier.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                products = (sortBy ?? "carbon_asc").ToLowerInvariant() switch
                {
                    "carbon_desc" => products.OrderByDescending(product => product.CarbonScore).ThenBy(product => product.Name).ToList(),
                    "price_asc" => products.OrderBy(product => product.Price).ThenBy(product => product.Name).ToList(),
                    "price_desc" => products.OrderByDescending(product => product.Price).ThenBy(product => product.Name).ToList(),
                    "name_asc" => products.OrderBy(product => product.Name).ToList(),
                    "badge_asc" => products.OrderBy(product => GetEcoTier(product.CarbonScore)).ThenBy(product => product.CarbonScore).ToList(),
                    _ => products.OrderBy(product => product.CarbonScore).ThenBy(product => product.Name).ToList()
                };

                var viewModel = new EcoCatalogViewModel
                {
                    Search = search?.Trim() ?? string.Empty,
                    MaxBudget = maxBudget,
                    SortBy = string.IsNullOrWhiteSpace(sortBy) ? "carbon_asc" : sortBy,
                    Tier = tier?.Trim() ?? string.Empty,
                    DebugMessage = $"Loaded {products.Count} eco product(s).",
                    Products = products
                };

                return View("~/Views/Module3/P2-5/EcoCatalog.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                var viewModel = new EcoCatalogViewModel
                {
                    Search = search?.Trim() ?? string.Empty,
                    MaxBudget = maxBudget,
                    SortBy = string.IsNullOrWhiteSpace(sortBy) ? "carbon_asc" : sortBy,
                    Tier = tier?.Trim() ?? string.Empty,
                    DebugMessage = $"Feature 5 failed: {ex.Message}"
                };

                return View("~/Views/Module3/P2-5/EcoCatalog.cshtml", viewModel);
            }
        }

        private static string GetEcoTier(decimal carbonScore)
        {
            return carbonScore switch
            {
                <= 120m => "Gold",
                <= 180m => "Silver",
                <= 250m => "Bronze",
                _ => "Standard"
            };
        }
    }
}
