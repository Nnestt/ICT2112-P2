using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Controls;

namespace ProRental.Controllers.Module1;

/// <summary>
/// ASP.NET HTTP controller for Module 1 (authentication, session, customer validation).
/// This is a thin HTTP boundary only — all business logic lives in the Control classes.
/// </summary>
public class Module1Controller : Controller
{
    private readonly AuthenticationControl _authControl;
    private readonly CustomerIDValidationControl _customerIdValidationControl;

    public Module1Controller(
        AuthenticationControl authControl,
        CustomerIDValidationControl customerIdValidationControl)
    {
        _authControl = authControl;
        _customerIdValidationControl = customerIdValidationControl;
    }

    // ── Login ────────────────────────────────────────────────────────────

    // GET /Module1/Login
    public IActionResult Login()
    {
        return View("P2-6/Login");
    }

    // POST /Module1/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(int userId, string password)
    {
        var result = _authControl.AuthenticateUser(userId, password);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Login failed.");
            return View("P2-6/Login");
        }

        // Store sessionId in an HTTP session cookie so subsequent requests can validate it.
        HttpContext.Session.SetInt32("SessionId", result.Session!.SessionId);
        return RedirectToAction("Index", "Home");
    }

    // POST /Module1/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var sessionId = HttpContext.Session.GetInt32("SessionId");
        if (sessionId.HasValue)
            _authControl.Logout(sessionId.Value);

        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    // ── Customer ID Validation ───────────────────────────────────────────

    // GET /Module1/CustomerIdEntry
    public IActionResult CustomerIdEntry()
    {
        return View("P2-6/_CustomerIdEntry");
    }

    // POST /Module1/CustomerIdEntry
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CustomerIdEntry(int customerId)
    {
        var result = _customerIdValidationControl.ValidateCustomer(customerId);

        if (!result.IsValid)
        {
            ViewBag.ValidationMessage = result.ValidationMessage;
            return View("P2-6/_CustomerIdEntry");
        }

        // Store the validated customer ID for the downstream checkout flow.
        HttpContext.Session.SetInt32("ValidatedCustomerId", result.CustomerId);
        return RedirectToAction("Index", "Cart");
    }

    // ── Staff Login ──────────────────────────────────────────────────────

    // GET /Module1/StaffLogin
    public IActionResult StaffLogin()
    {
        return View("P2-6/StaffLogin");
    }

    // POST /Module1/StaffLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StaffLogin(int staffUserId, string staffPassword)
    {
        var result = _authControl.AuthenticateUser(staffUserId, staffPassword);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Staff login failed.");
            return View("P2-6/StaffLogin");
        }

        HttpContext.Session.SetInt32("SessionId", result.Session!.SessionId);
        return RedirectToAction("Index", "Home");
    }
}