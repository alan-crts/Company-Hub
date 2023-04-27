namespace CompanyHub.Services.Service;

public interface IServiceService
{
    Task<List<Models.Service>> ToList();
    Task<List<Models.Service>> ListWithSearch(string? search, int page);
    Task<int> CountWithSearch(string? search);
    Task<Models.Service?> GetById(int id);
    Task Create(Models.Service service);
    Task Update(Models.Service service);
    Task Delete(Models.Service service);
}