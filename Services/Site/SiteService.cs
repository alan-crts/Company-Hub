using CompanyHub.Context;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services.Site;

public class SiteService : ISiteService
{
    private readonly MainContext _context;

    public SiteService(MainContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Site>> ToList()
    {
        return await _context.Site
            .Include(s => s.Employees)
            .ToListAsync();
    }

    public async Task<List<Models.Site>> ListWithSearch(string? search, int page)
    {
        if (search != null)
        {
            search = search.Trim();

            return await _context.Site
                .Include(s => s.Employees)
                .Where(
                    s => s.City.ToLower().Contains(search.ToLower())
                )
                .Skip((page - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        return await _context.Site
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

            return await _context.Site
                .Include(s => s.Employees)
                .Where(
                    s => s.City.ToLower().Contains(search.ToLower())
                )
                .CountAsync();
        }

        return await _context.Site
            .Include(s => s.Employees)
            .CountAsync();
    }

    public async Task<Models.Site?> GetById(int id)
    {
        return await _context.Site
            .Include(s => s.Employees)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task Create(Models.Site site)
    {
        await _context.Site.AddAsync(site);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Models.Site site)
    {
        var currentSite = await _context.Site.FirstOrDefaultAsync(s => s.Id == site.Id);

        currentSite.City = site.City;
        currentSite.Description = site.Description;

        _context.Site.Update(currentSite);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Models.Site site)
    {
        _context.Site.Remove(site);
        await _context.SaveChangesAsync();
    }
}