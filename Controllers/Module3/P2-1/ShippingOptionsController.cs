using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces.Domain;
using ProRental.Models.Module3.P2_1;

namespace ProRental.Controllers;

public sealed class ShippingOptionsController : Controller
{
    private readonly IShippingOptionService _shippingOptionService;
    private readonly IRankingService _rankingService;

    public ShippingOptionsController(IShippingOptionService shippingOptionService, IRankingService rankingService)
    {
        _shippingOptionService = shippingOptionService;
        _rankingService = rankingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetShippingOptions(int orderId, CancellationToken cancellationToken)
    {
        var options = await _shippingOptionService.GetShippingOptionsForOrderAsync(orderId, cancellationToken);
        ViewData["OrderId"] = orderId;
        return View("Index", options);
    }

    [HttpGet]
    public async Task<IActionResult> CompareOptions(int orderId, CancellationToken cancellationToken)
    {
        var options = await _shippingOptionService.GetShippingOptionsForOrderAsync(orderId, cancellationToken);
        ViewData["OrderId"] = orderId;
        ViewData["SpeedRanked"] = _rankingService.RankBySpeed(options);
        ViewData["CostRanked"] = _rankingService.RankByCost(options);
        ViewData["CarbonRanked"] = _rankingService.RankByCarbon(options);

        return View("Compare", options);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectShippingOption(int orderId, int optionId, CancellationToken cancellationToken)
    {
        var result = await _shippingOptionService.ApplyCustomerSelectionAsync(
            new SelectShippingOptionRequest(orderId, optionId),
            cancellationToken);

        return View("Selected", result);
    }
}
