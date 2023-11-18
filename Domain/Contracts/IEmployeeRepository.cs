namespace Domain.Contracts;

public interface IEmployeeRepository
{
    public Task AddEmployee(Employee employee);

    public Task<Employee?> GetEmployee(Guid id);

    public Task<List<Employee>> GetAllEmployees();

    public Task<List<Tuple<string?, string?>>> GetFilterNames();

    public Task UpdateEmployee(Employee employee);

    public Task DeleteEmployee(Employee employee);
}