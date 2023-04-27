using CompanyHub.Context;
using CompanyHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class SiteController : Controller
{
    private readonly MainContext _context;

    public SiteController(MainContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        List<Site> sites;
        if (search != null)
        {
            search = search.Trim();
            
            sites = await _context.Site
                .Include(e => e.Employees)
                .Where(
                    s => s.City.ToLower().Contains(search.ToLower())
                )
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();
            
            ViewBag.Search = search;
            ViewBag.SiteCount = _context.Site
                .Count(s => s.City.ToLower().Contains(search.ToLower()));
        }
        else
        {
            sites = await _context.Site
                .Include(e => e.Employees)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();
            
            ViewBag.Search = "";
            ViewBag.SiteCount = _context.Site.Count();
        }
        ViewBag.PageCount = Math.Ceiling((double) ViewBag.SiteCount / 10);
        if (page > ViewBag.PageCount && ViewBag.PageCount != 0) return RedirectToAction("Index", new { page = ViewBag.PageCount, search });
        
        ViewBag.Page = page;
        return View(sites);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Site/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var site = await _context.Site.FindAsync(id);
        if (site == null) return RedirectToAction("Index");
        
        return View(site);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Site site)
    {
        if (!ModelState.IsValid) return RedirectToAction("Add");
        
        await _context.Site.AddAsync(site);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Site site)
    {
        if (!ModelState.IsValid) return RedirectToAction("Edit", new { id = site.Id });

        Site? currentSite = await _context.Site.FindAsync(site.Id);
        if (currentSite == null) return NotFound();
        
        currentSite.City = site.City;
        currentSite.Description = site.Description;
        
        _context.Site.Update(currentSite);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var site = await _context.Site.FindAsync(id);
        if (site == null) return NotFound();
        
        if(site.Employees?.Count > 0) return RedirectToAction("Index");

        _context.Site.Remove(site);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}