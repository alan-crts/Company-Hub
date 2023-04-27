using CompanyHub.Models;
using CompanyHub.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers;

public class ServiceController : Controller
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewBag.ServiceCount = await _serviceService.CountWithSearch(search);

        ViewBag.PageCount = Math.Ceiling((double)ViewBag.ServiceCount / 10);
        if (page > ViewBag.PageCount && ViewBag.PageCount != 0)
            return RedirectToAction("Index", new { page = ViewBag.PageCount, search });

        var services = await _serviceService.ListWithSearch(search, page);
        ViewBag.Search = search;
        ViewBag.Page = page;

        return View(services);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Service/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _serviceService.GetById(id);
        if (service == null) return NotFound();

        return View(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Service service)
    {
        if (!ModelState.IsValid) return View(service);

        await _serviceService.Create(service);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Service service)
    {
        if (!ModelState.IsValid) return RedirectToAction("Edit", new { id = service.Id });

        var currentService = await _serviceService.GetById(service.Id);
        if (currentService == null) return NotFound();

        await _serviceService.Update(service);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _serviceService.GetById(id);
        if (service == null) return NotFound();

        if (service.Employees?.Count > 0) return RedirectToAction("Index");

        await _serviceService.Delete(service);

        return RedirectToAction("Index");
    }
}