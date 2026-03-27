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
        private readonly EcoBadgeControl _ecoBadgeControl;

        public CatalogPageController(ICatalogGateway catalogGateway)
        {
            _control = new CatalogControl(catalogGateway);
            _ecoBadgeControl = new EcoBadgeControl();
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
                            product.GetName().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            product.GetDescription().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            product.GetEcoBadge().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            _ecoBadgeControl.GetBadge(_ecoBadgeControl.AssignTier(product.GetCarbonScore())).Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (maxBudget.HasValue)
                {
                    products = products
                        .Where(product => product.GetPrice() <= maxBudget.Value)
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(tier))
                {
                    if (!Enum.TryParse<EcoTier>(tier.Trim(), true, out var selectedTier))
                    {
                        selectedTier = EcoTier.Standard;
                    }

                    products = products
                        .Where(product => _ecoBadgeControl.AssignTier(product.GetCarbonScore()) == selectedTier)
                        .ToList();
                }

                products = (sortBy ?? "carbon_asc").ToLowerInvariant() switch
                {
                    "carbon_desc" => products.OrderByDescending(product => product.GetCarbonScore()).ThenBy(product => product.GetName()).ToList(),
                    "price_asc" => products.OrderBy(product => product.GetPrice()).ThenBy(product => product.GetName()).ToList(),
                    "price_desc" => products.OrderByDescending(product => product.GetPrice()).ThenBy(product => product.GetName()).ToList(),
                    "name_asc" => products.OrderBy(product => product.GetName()).ToList(),
                    "badge_asc" => products.OrderBy(product => _ecoBadgeControl.AssignTier(product.GetCarbonScore())).ThenBy(product => product.GetCarbonScore()).ToList(),
                    _ => products.OrderBy(product => product.GetCarbonScore()).ThenBy(product => product.GetName()).ToList()
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
    }
}
