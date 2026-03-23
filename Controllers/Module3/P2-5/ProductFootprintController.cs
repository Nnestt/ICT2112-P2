using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5;

public sealed class ProductFootprintController : Controller
{
    private readonly IProductFootprintCalculatorService _productFootprintCalculatorService;

    public ProductFootprintController(IProductFootprintCalculatorService productFootprintCalculatorService)
    {
        _productFootprintCalculatorService = productFootprintCalculatorService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new ProductFootprintCalculationViewModel();
        TryPopulateProductOptions(model);
        return View("~/Views/Module3/P2-5/ProductFootprintCalculator.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ProductFootprintCalculationViewModel model)
    {
        TryPopulateProductOptions(model);

        if (!ModelState.IsValid)
        {
            return View("~/Views/Module3/P2-5/ProductFootprintCalculator.cshtml", model);
        }

        try
        {
            model.CarbonFootprint = _productFootprintCalculatorService.CalculateProductFootprint(
                model.ProductMass!.Value,
                model.ToxicPercentage!.Value);

            var result = _productFootprintCalculatorService.CalculateAndStoreFootprint(
                model.ProductId!.Value,
                model.ProductMass!.Value,
                model.ToxicPercentage!.Value);

            model.CarbonFootprint = result.CarbonFootprint;
            model.CalculatedAt = result.CalculatedAt;
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (DbUpdateException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                $"The footprint was calculated but could not be saved to the database: {exception.GetBaseException().Message}");
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(
                string.Empty,
                $"The footprint was calculated but an unexpected error occurred while saving: {exception.GetBaseException().Message}");
        }

        return View("~/Views/Module3/P2-5/ProductFootprintCalculator.cshtml", model);
    }

    private void TryPopulateProductOptions(ProductFootprintCalculationViewModel model)
    {
        try
        {
            model.ProductOptions = _productFootprintCalculatorService
                .GetProductDropdownItems()
                .Select(product => new SelectListItem
                {
                    Value = product.ProductId.ToString(),
                    Text = product.ProductName,
                    Selected = model.ProductId == product.ProductId
                })
                .ToList();
        }
        catch (Exception exception)
        {
            model.ProductOptions = [];
            ModelState.AddModelError(string.Empty, $"Unable to load products: {exception.Message}");
        }
    }
}
