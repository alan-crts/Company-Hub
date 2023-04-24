using CompanyHub.Context;
using CompanyHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Controllers;

public class EmployeeController : Controller
{
     private readonly MainContext _context;

     public EmployeeController(MainContext context)
     {
          _context = context;
     }
     //return list of employees
     public IActionResult Index()
     {
          List<Employee> employees = _context.Employee
               .Include(e => e.Service)
               .Include(e => e.Site)
               .ToList();
          return View(employees);
     }
}