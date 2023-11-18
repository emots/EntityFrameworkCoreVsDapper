using Domain;

namespace DataAccessEFCore.Interfaces;

public interface IEmployeeRepositoryEfExtend
{
    public Task<Employee?> GetEmployeeAsNoTracking(Guid id);

    public Task<Employee?> GetEmployeeWithRawSQL(Guid id);

    public Task<Employee?> GetEmployeeCompiled(Guid id);

    public Task<List<Employee>> GetAllEmployeesAsNoTracking();

    public Task<List<Employee>> GetAllEmployeesWithRawSQL();

    public Task<List<Employee>> GetAllEmployeesCompiled();

    public Task ExecuteUpdateEmployee(Employee employee);

    public Task ExecuteDeleteEmployee(Guid id);
}