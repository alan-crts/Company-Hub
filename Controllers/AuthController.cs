using System.Security.Claims;
using CompanyHub.Context;
using CompanyHub.Models;
using CompanyHub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class AuthController : Controller
{
    private readonly MainContext _context;
    private readonly IHashService _hashService;

    public AuthController(MainContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }
    
    public IActionResult Login()
    {
        if(User.Identity is { IsAuthenticated: true }) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login login)
    {
        if (!ModelState.IsValid) return View(login);

        Employee? user = await _context.Employee.FirstOrDefaultAsync(u =>
            u.Email == login.Email && u.Password == _hashService.GetHash(login.Password));
        if (user == null)
        {
            ModelState.AddModelError("Email", "L'adresse email ou le mot de passe est incorrect.");
            ModelState.AddModelError("Password", "L'adresse email ou le mot de passe est incorrect.");
            return View(login);
        }

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
        
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
        return RedirectToAction("Login", "Auth");
    }
    
    public IActionResult PasswordModification()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult PasswordModification([FromForm] PasswordModification passwordModification)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        Employee? currentEmployee = _context.Employee.Find(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        if (currentEmployee == null) return NotFound();

        if (currentEmployee.Password != _hashService.GetHash(passwordModification.OldPassword))
        {
            ModelState.AddModelError("OldPassword", "L'ancien mot de passe est incorrect.");
            return View(passwordModification);
        }

        currentEmployee.Password = _hashService.GetHash(passwordModification.NewPassword);
        _context.Employee.Update(currentEmployee);
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }
}