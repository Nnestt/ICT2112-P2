using Microsoft.AspNetCore.Mvc;
using ProRental.Interfaces.Domain;

namespace ProRental.Controllers;

public class RewardsDashboardController : Controller
{
    private readonly IRewardsControl _rewardsControl;

    public RewardsDashboardController(IRewardsControl rewardsControl)
    {
        _rewardsControl = rewardsControl;
    }

    // ── DisplayRewards ────────────────────────────────────────────────────────
    public IActionResult DisplayRewards()
    {
        ViewBag.Orders        = _rewardsControl.GetAllOrders();
        ViewBag.CarbonRecords = _rewardsControl.GetAllCarbonRecords();
        ViewBag.Rewards       = _rewardsControl.GetAllRewards();
        return View("~/Views/Module3/P2-5/RewardsDashboardView.cshtml");
    }

    // ── CalculateEcoScore ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CalculateEcoScore(int orderId)
    {
        int score = _rewardsControl.CalculateEcoScore(orderId);

        TempData["ScoreMessage"] = score < 0
            ? $"No carbon data found for Order #{orderId}. Process the order first."
            : $"Order #{orderId} — Eco Score: {score}/100";

        return RedirectToAction(nameof(DisplayRewards));
    }

    // ── DetermineReward ───────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DetermineReward(int orderId)
    {
        var reward = _rewardsControl.DetermineReward(orderId);

        TempData["RewardMessage"] = reward is not null
            ? $"Reward issued for Order #{orderId}: {reward.GetFormattedValue()}"
            : $"Order #{orderId} does not qualify for a reward (impact too high).";

        return RedirectToAction(nameof(DisplayRewards));
    }

    // ── ProcessOrder ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessOrder(int orderId)
    {
        var carbonData = _rewardsControl.CreateOrderCarbonData(orderId, 0);

        TempData["SuccessMessage"] = $"Order #{orderId} processed. " +
            $"Total carbon: {carbonData.GetTotalcarbon():F2}g — Impact: {carbonData.GetImpactlevel()}";

        return RedirectToAction(nameof(DisplayRewards));
    }

    // ── MyRewards ─────────────────────────────────────────────────────────────
    // Customer-facing page — shows all issued rewards
    // TODO: filter by logged-in customerId once auth is integrated
    public IActionResult MyRewards()
    {
        var rewards = _rewardsControl.GetAllRewards();
        return View("~/Views/Module3/P2-5/MyRewardsView.cshtml", rewards);
    }
}