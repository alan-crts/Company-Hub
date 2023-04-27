using System.Security.Claims;
using CompanyHub.Context;
using CompanyHub.Models;
using CompanyHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class EmployeeController : Controller
{
    private readonly MainContext _context;
    private readonly IHashService _hashService;

    public EmployeeController(MainContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    [HttpGet]
    public IActionResult Index(string? search, int page = 1, int? service = null, int? site = null)
    {
        IQueryable<Employee> employees = _context.Employee
            .Include(e => e.Service)
            .Include(e => e.Site);
        if (search != null)
        {
            search = search.Trim();
            employees = employees.Where(
                e => e.FirstName.ToLower().Contains(search.ToLower())
                     || e.LastName.ToLower().Contains(search.ToLower())
                     || e.Email.ToLower().Contains(search.ToLower())
                     || e.LandlinePhone.ToLower().Contains(search.ToLower())
                     || e.MobilePhone.ToLower().Contains(search.ToLower())
            );
            
            ViewBag.Search = search;
        }
        else
        {
            ViewBag.Search = "";
        }
        
        if(service != null)
        {
            employees = employees.Where(e => e.ServiceId == service);
            ViewBag.Service = service;
        }
        else
        {
            ViewBag.Service = null;
        }
        
        if(site != null)
        {
            employees = employees.Where(e => e.SiteId == site);
            ViewBag.Site = site;
        }
        else
        {
            ViewBag.Site = null;
        }
        
        ViewBag.EmployeeCount = employees.Count();
        employees = employees.Skip((page - 1) * 10).Take(10);
        
        ViewBag.PageCount = Math.Ceiling((decimal)ViewBag.EmployeeCount / 10);
        if (page > ViewBag.PageCount && ViewBag.PageCount != 0) return RedirectToAction("Index", new { page = ViewBag.PageCount, search });
        
        List<Service> services = _context.Service.ToList();
        List<Site> sites = _context.Site.ToList();
        
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        ViewBag.Page = page;
        
        return View(employees.ToList());
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        List<Service> services = _context.Service.ToList();
        List<Site> sites = _context.Site.ToList();
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/Employee/Edit/{id}")]
    public IActionResult Edit(int id)
    {
        Employee? employee = _context.Employee.Find(id);
        if (employee == null) return NotFound();
        List<Service> services = _context.Service.ToList();
        List<Site> sites = _context.Site.ToList();
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        return View(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] Employee employee)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        Employee? existingEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == employee.Email);
        if (existingEmployee != null)
        {
            ModelState.AddModelError("Email", "L'adresse email est déjà utilisée.");
            List<Service> services = _context.Service.ToList();
            List<Site> sites = _context.Site.ToList();
            ViewBag.Services = services;
            ViewBag.Sites = sites;
            return View(employee);
        }

        await _context.Employee.AddAsync(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromForm] Employee employee)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        Employee? currentEmployee = await _context.Employee.FindAsync(employee.Id);
        if (currentEmployee == null) return NotFound();

        currentEmployee.FirstName = employee.FirstName;
        currentEmployee.LastName = employee.LastName;
        currentEmployee.Email = employee.Email;
        currentEmployee.LandlinePhone = employee.LandlinePhone;
        currentEmployee.MobilePhone = employee.MobilePhone;
        currentEmployee.ServiceId = employee.ServiceId;
        currentEmployee.SiteId = employee.SiteId;
        if (currentEmployee.Id.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier)) currentEmployee.IsAdmin = employee.IsAdmin;
        if (employee.Password != null && string.IsNullOrEmpty(currentEmployee.Password))
            currentEmployee.Password = _hashService.GetHash(employee.Password);
        
        _context.Employee.Update(currentEmployee);
        
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        Employee? employee = await _context.Employee.FindAsync(id);
        if (employee == null) return NotFound();
        
        if (employee.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)) return RedirectToAction("Index");
        
        _context.Employee.Remove(employee);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email, int id)
    {
        Employee? employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == email && e.Id != id);
        if (employee != null) return Json(false);
        return Json(true);
    }
}