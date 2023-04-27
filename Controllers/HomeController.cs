using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}