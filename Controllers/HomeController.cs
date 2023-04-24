using System.Diagnostics;
using System.Security.Claims;
using CompanyHub.Context;
using CompanyHub.Models;
using CompanyHub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class HomeController : Controller
{
    private readonly MainContext _context;
    private readonly IHashService _hashService;

    public HomeController(MainContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        if(User.Identity is { IsAuthenticated: true }) return RedirectToAction("Index", "Employee");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login login)
    {
        if (!ModelState.IsValid) return View(login);

        Employee? user = await _context.Employee.FirstOrDefaultAsync(u =>
            u.Email == login.Email && u.Password == _hashService.GetHash(login.Password));
        if (user == null) return RedirectToAction("Login", "Home");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Visitor")
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(claimsIdentity);
        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            props).Wait();

        if (Request.Query.ContainsKey("ReturnUrl"))
            return Redirect(Request.Query["ReturnUrl"]!);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
        return Redirect(Request.Headers["Referer"].ToString());
    }
}