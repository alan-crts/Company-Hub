using CompanyHub.Context;
using CompanyHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class ServiceController : Controller
{
    private readonly MainContext _context;

    public ServiceController(MainContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Index(string? search, int page = 1)
    {
        List<Service> services;
        if (search != null)
        {
            search = search.Trim();
            
            services = _context.Service
                .Include(s => s.Employees)
                .Where(
                    s => s.Name.ToLower().Contains(search.ToLower())
                )
                .Skip((page - 1) * 10)
                .Take(10)
                .ToList();
            ViewBag.Search = search;
            ViewBag.ServiceCount = _context.Service
                .Count(s => s.Name.ToLower().Contains(search.ToLower()));
        }
        else
        {
            services = _context.Service
                .Include(s => s.Employees)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToList();
            ViewBag.Search = "";
            ViewBag.ServiceCount = _context.Service.Count();
        }
        ViewBag.PageCount = Math.Ceiling((double) ViewBag.ServiceCount / 10);
        if (page > ViewBag.PageCount && ViewBag.PageCount != 0) return RedirectToAction("Index", new { page = ViewBag.PageCount, search });
        
        ViewBag.Page = page;
        return View(services);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Service/Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var service = await _context.Service.FindAsync(id);
        if (service == null) return NotFound();

        return View(service);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Service service)
    {
        if (!ModelState.IsValid) return RedirectToAction("Add");
        
        await _context.Service.AddAsync(service);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Service service)
    {
        if (!ModelState.IsValid) return RedirectToAction("Edit", new { id = service.Id });

        Service? currentService = await _context.Service.FindAsync(service.Id);
        if (currentService == null) return NotFound();
        
        currentService.Name = service.Name;
        
        _context.Service.Update(currentService);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        Service? service = await _context.Service.FindAsync(id);
        if (service == null) return NotFound();
        
        if(service.Employees?.Count > 0) return RedirectToAction("Index");
        
        _context.Service.Remove(service);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}