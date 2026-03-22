using Microsoft.AspNetCore.Mvc;
using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Controllers.Module3.P2_1;

[Route("module3/p2-1/transport-carbon")]
public class TransportCarbonController : Controller
{
    private readonly ITransportCarbonService _transportCarbonService;
    private readonly IShippingOptionService _shippingOptionService;
    private readonly IHubCarbonService _hubCarbonService;
    private readonly IRouteDistanceCalculator _routeDistanceCalculator;
    private readonly IPricingRuleGateway _pricingRuleGateway;

    public TransportCarbonController(
        ITransportCarbonService transportCarbonService,
        IShippingOptionService shippingOptionService,
        IHubCarbonService hubCarbonService,
        IRouteDistanceCalculator routeDistanceCalculator,
        IPricingRuleGateway pricingRuleGateway)
    {
        _transportCarbonService = transportCarbonService;
        _shippingOptionService = shippingOptionService;
        _hubCarbonService = hubCarbonService;
        _routeDistanceCalculator = routeDistanceCalculator;
        _pricingRuleGateway = pricingRuleGateway;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Module3/P2-1/TransportCarbonTest.cshtml");
    }

    [HttpGet("test")]
    public IActionResult Test(int shippingOptionId, double weightKg)
    {
        try
        {
            var carbonInput = _shippingOptionService.GetRouteCarbonInput(shippingOptionId);
            var storageCo2 = _hubCarbonService.CalculateProductStorageCarbon(carbonInput.ProductId, carbonInput.HubId);
            var legCalculations = carbonInput.RouteLegs.Select(routeLeg =>
            {
                var distanceKm = _routeDistanceCalculator.CalculateDistanceKm(routeLeg.StartPoint, routeLeg.EndPoint);
                var rule = _pricingRuleGateway.FindByTransportMode(routeLeg.Mode)
                    .FirstOrDefault(r => r.ReadIsActive());
                var baseRate = (double)(rule?.ReadBaseRatePerKm() ?? 0m);
                var surchargeRate = (double)(rule?.ReadCarbonSurcharge() ?? 0m);
                var legCarbonBase = _transportCarbonService.CalculateLegCarbon(
                    carbonInput.Quantity,
                    weightKg,
                    distanceKm,
                    storageCo2);
                var legCarbonValue = legCarbonBase * baseRate;
                var legCarbonSurcharge = _transportCarbonService.CalculateCarbonSurcharge(legCarbonValue, surchargeRate);

                return new
                {
                    mode = routeLeg.Mode.ToString(),
                    routeLeg.StartPoint,
                    routeLeg.EndPoint,
                    distanceKm,
                    baseRate,
                    surchargeRate,
                    legCarbonBase,
                    legCarbonValue,
                    legCarbonSurcharge
                };
            }).ToList();

            var legCarbonValues = legCalculations.Select(leg => (double)leg.legCarbonValue).ToList();
            var routeCarbonKg = _transportCarbonService.CalculateRouteCarbon(legCarbonValues);
            var carbonSurchargeKg = legCalculations.Sum(leg => (double)leg.legCarbonSurcharge);

            return Json(new
            {
                shippingOptionId,
                weightKg,
                quantity = carbonInput.Quantity,
                productId = carbonInput.ProductId,
                hubId = carbonInput.HubId,
                storageCo2,
                legCalculations,
                routeCarbonKg,
                carbonSurchargeKg,
                totalCarbonKg = routeCarbonKg + carbonSurchargeKg
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                shippingOptionId,
                weightKg,
                error = ex.Message
            });
        }
    }
}
