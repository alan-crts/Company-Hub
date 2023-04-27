namespace CompanyHub.Services.Employee;

public interface IEmployeeService
{
    Task<List<Models.Employee>> ListWithFilter(Dictionary<string, string> filters, int page = 1);
    Task<Models.Employee?> GetById(int id);
    Task<Models.Employee?> GetByEmail(string email);
    Task<int> CountWithFilter(Dictionary<string, string> filters);
    Task Create(Models.Employee employee);
    Task Update(Models.Employee employee);
    Task Delete(Models.Employee employee);
    Task<bool> CheckEmail(string email, int id);
}