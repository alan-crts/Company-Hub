using CompanyHub.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly MainContext _context;
    private readonly IHashService _hashService;

    public EmployeeService(MainContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    public async Task<List<Models.Employee>> ListWithFilter(Dictionary<string, string> filters, int page = 1)
    {
        IQueryable<Models.Employee> employees = _context.Employee
            .Include(e => e.Service)
            .Include(e => e.Site);
        if (filters.ContainsKey("search"))
        {
            var search = filters["search"];
            search = search.Trim();
            employees = employees.Where(
                e => e.FirstName.ToLower().Contains(search.ToLower())
                     || e.LastName.ToLower().Contains(search.ToLower())
                     || e.Email.ToLower().Contains(search.ToLower())
                     || e.LandlinePhone.ToLower().Contains(search.ToLower())
                     || e.MobilePhone.ToLower().Contains(search.ToLower())
            );
        }

        if (filters.ContainsKey("service"))
        {
            var service = int.Parse(filters["service"]);
            employees = employees.Where(e => e.ServiceId == service);
        }

        if (filters.ContainsKey("site"))
        {
            var site = int.Parse(filters["site"]);
            employees = employees.Where(e => e.SiteId == site);
        }

        employees = employees.Skip((page - 1) * 10).Take(10);

        return await employees.ToListAsync();
    }

    public async Task<Models.Employee?> GetById(int id)
    {
        return _context.Employee
            .Include(e => e.Service)
            .Include(e => e.Site)
            .FirstOrDefault(e => e.Id == id);
    }

    public async Task<Models.Employee?> GetByEmail(string email)
    {
        return _context.Employee
            .Include(e => e.Service)
            .Include(e => e.Site)
            .FirstOrDefault(e => e.Email == email);
    }

    public async Task<int> CountWithFilter(Dictionary<string, string> filters)
    {
        IQueryable<Models.Employee> employees = _context.Employee;
        if (filters.ContainsKey("search"))
        {
            var search = filters["search"];
            search = search.Trim();
            employees = employees.Where(
                e => e.FirstName.ToLower().Contains(search.ToLower())
                     || e.LastName.ToLower().Contains(search.ToLower())
                     || e.Email.ToLower().Contains(search.ToLower())
                     || e.LandlinePhone.ToLower().Contains(search.ToLower())
                     || e.MobilePhone.ToLower().Contains(search.ToLower())
            );
        }

        if (filters.ContainsKey("service"))
        {
            var service = int.Parse(filters["service"]);
            employees = employees.Where(e => e.ServiceId == service);
        }

        if (filters.ContainsKey("site"))
        {
            var site = int.Parse(filters["site"]);
            employees = employees.Where(e => e.SiteId == site);
        }

        return await employees.CountAsync();
    }

    public async Task Create(Models.Employee employee)
    {
        await _context.Employee.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Models.Employee employee)
    {
        var currentEmployee = await _context.Employee.FindAsync(employee.Id);

        currentEmployee.FirstName = employee.FirstName;
        currentEmployee.LastName = employee.LastName;
        currentEmployee.Email = employee.Email;
        currentEmployee.LandlinePhone = employee.LandlinePhone;
        currentEmployee.MobilePhone = employee.MobilePhone;
        currentEmployee.ServiceId = employee.ServiceId;
        currentEmployee.SiteId = employee.SiteId;
        currentEmployee.IsAdmin = employee.IsAdmin;
        if (employee.Password != null && string.IsNullOrEmpty(currentEmployee.Password))
            currentEmployee.Password = _hashService.GetHash(employee.Password);

        _context.Employee.Update(currentEmployee);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(Models.Employee employee)
    {
        _context.Employee.Remove(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckEmail(string email, int id)
    {
        var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == email && e.Id != id);
        if (employee != null) return false;
        return true;
    }
}