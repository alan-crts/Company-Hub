using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers;

public class ErrorController : Controller
{
    [Route("/Error/{statusCode:int}")]
    public IActionResult Index(int? statusCode)
    {
        if (statusCode == null) return View();

        return statusCode switch
        {
            404 => View("NotFound"),
            403 => View("Forbidden"),
            500 => View("InternalServerError"),
            400 => View("BadRequest"),
            401 => View("Unauthorized"),
            _ => View()
        };
    }
}