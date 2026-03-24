using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Enums;
using ProRental.Interfaces.Module3.P2_1;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Controllers;

/// <summary>
/// HTTP boundary for Feature 1. It coordinates the shipping-option flow and keeps
/// ranking, persistence, routing, and carbon logic behind service contracts.
/// by: ernest
/// </summary>
public sealed class ShippingOptionsController : Controller
{
    private const string ViewRoot = "~/Views/Module3/P2-1/ShippingOptions/";
    private readonly IShippingOptionService _shippingOptionService;

    public ShippingOptionsController(IShippingOptionService shippingOptionService)
    {
        _shippingOptionService = shippingOptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetShippingOptions(int orderId, CancellationToken cancellationToken)
    {
        var options = await _shippingOptionService.GetPreferenceChoicesForOrderAsync(orderId, cancellationToken);
        ViewData["OrderId"] = orderId;
        return View($"{ViewRoot}Index.cshtml", options);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectShippingPreference(int orderId, PreferenceType preferenceType, CancellationToken cancellationToken)
    {
        var result = await _shippingOptionService.ConfirmPreferenceSelectionAsync(
            new SelectShippingPreferenceRequest(orderId, preferenceType),
            cancellationToken);

        return View($"{ViewRoot}Selected.cshtml", result);
    }
}
