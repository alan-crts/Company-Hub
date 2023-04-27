using CompanyHub.Models;
using CompanyHub.Services.Site;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers;

public class SiteController : Controller
{
    private readonly ISiteService _siteService;

    public SiteController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewBag.SiteCount = await _siteService.CountWithSearch(search);

        ViewBag.PageCount = Math.Ceiling((double)ViewBag.SiteCount / 10);
        if (page > ViewBag.PageCount && ViewBag.PageCount != 0)
            return RedirectToAction("Index", new { page = ViewBag.PageCount, search });

        var sites = await _siteService.ListWithSearch(search, page);
        ViewBag.Search = search;
        ViewBag.Page = page;

        return View(sites);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Site/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var site = await _siteService.GetById(id);
        if (site == null) return RedirectToAction("Index");

        return View(site);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Site site)
    {
        if (!ModelState.IsValid) return View(site);

        await _siteService.Create(site);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Site site)
    {
        if (!ModelState.IsValid) return RedirectToAction("Edit", new { id = site.Id });

        var currentSite = await _siteService.GetById(site.Id);
        if (currentSite == null) return NotFound();

        await _siteService.Update(site);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var site = await _siteService.GetById(id);
        if (site == null) return NotFound();

        if (site.Employees?.Count > 0) return RedirectToAction("Index");

        await _siteService.Delete(site);

        return RedirectToAction("Index");
    }
}