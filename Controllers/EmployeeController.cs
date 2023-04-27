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

    //return list of employees
    public IActionResult Index()
    {
        List<Employee> employees;
        if (Request.QueryString.Value.Contains("search"))
        {
            string search = Request.Query["search"];
            employees = _context.Employee
                .Include(e => e.Service)
                .Include(e => e.Site)
                .Where(
                    e => e.FirstName.ToLower().Contains(search.ToLower())
                         || e.LastName.ToLower().Contains(search.ToLower())
                         || e.Email.ToLower().Contains(search.ToLower())
                         || e.LandlinePhone.ToLower().Contains(search.ToLower())
                         || e.MobilePhone.ToLower().Contains(search.ToLower())
                )
                .ToList();
            @ViewBag.Search = search;
        }
        else
        {
            employees = _context.Employee
                .Include(e => e.Service)
                .Include(e => e.Site)
                .ToList();
            @ViewBag.Search = "";
        }

        List<Service> services = _context.Service.ToList();
        List<Site> sites = _context.Site.ToList();
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        return View(employees);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        List<Service> services = _context.Service.ToList();
        List<Site> sites = _context.Site.ToList();
        ViewBag.Services = services;
        ViewBag.Sites = sites;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        Employee employee = _context.Employee.Find(id);
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
            ModelState.AddModelError("Email", "This email address is already in use.");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        _context.Employee.Add(employee);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromForm] Employee employee)
    {
        if (!ModelState.IsValid) return Redirect(Request.Headers["Referer"].ToString());

        Employee currentEmployee = await _context.Employee.FindAsync(employee.Id);
        if (currentEmployee == null) return NotFound();

        currentEmployee.FirstName = employee.FirstName;
        currentEmployee.LastName = employee.LastName;
        currentEmployee.Email = employee.Email;
        currentEmployee.LandlinePhone = employee.LandlinePhone;
        currentEmployee.MobilePhone = employee.MobilePhone;
        currentEmployee.ServiceId = employee.ServiceId;
        currentEmployee.SiteId = employee.SiteId;
        if (currentEmployee.Id != employee.Id) currentEmployee.IsAdmin = employee.IsAdmin;
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
        Employee employee = await _context.Employee.FindAsync(id);
        if (employee == null) return NotFound();
        if (employee.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier)) return RedirectToAction("Index");
        _context.Employee.Remove(employee);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}