using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5;

public class CarbonMonitoringController : Controller
{
    private readonly ICarbonChartControl _carbonChartService;

    public CarbonMonitoringController(ICarbonChartControl carbonChartService)
    {
        _carbonChartService = carbonChartService;
    }

    public IActionResult CarbonDashboardView()
    {
        var dto = _carbonChartService.BuildDashboardDto();
        return View("~/Views/Module3/P2-5/CarbonDashboardView.cshtml", dto);
    }
}
