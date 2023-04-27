namespace CompanyHub.Services.Site;

public interface ISiteService
{
    Task<List<Models.Site>> ToList();
    Task<List<Models.Site>> ListWithSearch(string? search, int page);
    Task<int> CountWithSearch(string? search);
    Task<Models.Site?> GetById(int id);
    Task Create(Models.Site service);
    Task Update(Models.Site service);
    Task Delete(Models.Site service);
}