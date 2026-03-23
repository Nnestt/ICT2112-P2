using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5;

public class CarbonMonitoringController : Controller
{
    private readonly ICarbonChartService _carbonChartService;

    public CarbonMonitoringController(ICarbonChartService carbonChartService)
    {
        _carbonChartService = carbonChartService;
    }

    public IActionResult CarbonDashboardView()
    {
        var viewModel = _carbonChartService.BuildDashboardViewModel();
        return View("~/Views/Module3/P2-5/CarbonDashboardView.cshtml", viewModel);
    }
}
