using System.Security.Claims;
using CompanyHub.Models;
using CompanyHub.Services.Employee;
using CompanyHub.Services.Service;
using CompanyHub.Services.Site;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers;

public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IServiceService _serviceService;
    private readonly ISiteService _siteService;

    public EmployeeController(ISiteService siteService, IEmployeeService employeeService,
        IServiceService serviceService)
    {
        _siteService = siteService;
        _employeeService = employeeService;
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1, int? service = null, int? site = null)
    {
        Dictionary<string, string> filters = new();
        if (search != null) filters.Add("search", search);
        if (service != null) filters.Add("service", service.ToString());
        if (site != null) filters.Add("site", site.ToString());

        ViewBag.EmployeeCount = await _employeeService.CountWithFilter(filters);
        ViewBag.PageCount = Math.Ceiling((double)ViewBag.EmployeeCount / 10);

        if (page > ViewBag.PageCount && ViewBag.PageCount != 0)
            return RedirectToAction("Index", new { page = ViewBag.PageCount, search, service, site });

        var employees = await _employeeService.ListWithFilter(filters, page);

        ViewBag.Search = search;
        ViewBag.Service = service;
        ViewBag.Site = site;
        ViewBag.Services = await _serviceService.ToList();
        ViewBag.Sites = await _siteService.ToList();
        ViewBag.Page = page;

        return View(employees);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var services = await _serviceService.ToList();
        var sites = await _siteService.ToList();

        ViewBag.Services = services;
        ViewBag.Sites = sites;

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Employee/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetById(id);
        if (employee == null) return NotFound();

        var services = await _serviceService.ToList();
        var sites = await _siteService.ToList();
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        return View(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] Employee employee)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        var existingEmployee = await _employeeService.GetByEmail(employee.Email);
        if (existingEmployee != null)
        {
            ModelState.AddModelError("Email", "L'adresse email est déjà utilisée.");
            var services = await _serviceService.ToList();
            var sites = await _siteService.ToList();
            ViewBag.Services = services;
            ViewBag.Sites = sites;
            return View(employee);
        }

        await _employeeService.Create(employee);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromForm] Employee employee)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        var currentEmployee = await _employeeService.GetById(employee.Id);
        if (currentEmployee == null) return NotFound();

        await _employeeService.Update(employee);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetById(id);
        if (employee == null) return NotFound();

        if (employee.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)) return RedirectToAction("Index");

        await _employeeService.Delete(employee);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email, int id)
    {
        return Json(await _employeeService.CheckEmail(email, id));
    }
}