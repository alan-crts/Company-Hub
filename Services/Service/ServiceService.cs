using CompanyHub.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services.Service;

public class ServiceService : IServiceService
{
    private readonly MainContext _context;

    public ServiceService(MainContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Service>> ToList()
    {
        return await _context.Service
            .Include(s => s.Employees)
            .ToListAsync();
    }

    public async Task<List<Models.Service>> ListWithSearch(string? search, int page)
    {
        if (search != null)
        {
            search = search.Trim();

            return await _context.Service
                .Include(s => s.Employees)
                .Where(
                    s => s.Name.ToLower().Contains(search.ToLower())
                )
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        return await _context.Service
            .Include(s => s.Employees)
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync();
    }

    public async Task<int> CountWithSearch(string? search)
    {
        if (search != null)
        {
            search = search.Trim();

            return await _context.Service
                .Include(s => s.Employees)
                .Where(
                    s => s.Name.ToLower().Contains(search.ToLower())
                )
                .CountAsync();
        }

        return await _context.Service
            .Include(s => s.Employees)
            .CountAsync();
    }

    public async Task<Models.Service?> GetById(int id)
    {
        return await _context.Service
            .Include(s => s.Employees)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task Create(Models.Service service)
    {
        await _context.Service.AddAsync(service);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Models.Service service)
    {
        var currentService = await _context.Service.FirstOrDefaultAsync(s => s.Id == service.Id);

        currentService.Name = service.Name;

        _context.Service.Update(currentService);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Models.Service service)
    {
        _context.Service.Remove(service);
        await _context.SaveChangesAsync();
    }
}