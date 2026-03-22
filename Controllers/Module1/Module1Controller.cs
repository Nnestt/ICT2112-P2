using Microsoft.AspNetCore.Mvc;

namespace ProRental.Controllers.Module1;

public class Module1Controller : Controller
{
    // GET /Module1/Login
    public IActionResult Login()
    {
        return View("P2-6/Login");
    }

    // POST /Module1/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password, bool rememberMe = false)
    {
        // TODO: add authentication logic here
        return RedirectToAction("Index", "Home");
    }

    // GET /Module1/Signup
    public IActionResult Signup()
    {
        return View("P2-6/Signup");
    }

    // POST /Module1/Signup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Signup(string firstName, string lastName, string email,
                               string phone, string password, string confirmPassword,
                               bool agreeTerms = false)
    {
        // TODO: add registration logic here
        return RedirectToAction("Login");
    }

    // GET /Module1/StaffLogin
    public IActionResult StaffLogin()
    {
        return View("P2-6/StaffLogin");
    }

    // POST /Module1/StaffLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StaffLogin(string staffEmail, string staffPassword)
    {
        // TODO: add staff authentication logic here
        return RedirectToAction("Index", "Home");
    }
}