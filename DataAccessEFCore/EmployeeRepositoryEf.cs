using DataAccessEFCore.Interfaces;
using Domain;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEFCore;

public class EmployeeRepositoryEf : IEmployeeRepository, IEmployeeRepositoryEfExtend
{
    private static readonly Func<DataContext, Guid, Task<Employee?>> FirstOrDefaultEmployeeAsync =
        EF.CompileAsyncQuery((DataContext context, Guid id) 
            => context.Employees.FirstOrDefault(e => e.Id == id));

    private static readonly Func<DataContext, IAsyncEnumerable<Employee>> AllEmployeesAsync =
        EF.CompileAsyncQuery((DataContext context) 
            => context.Employees.Select(e => e));

    private readonly DataContext context;

    public EmployeeRepositoryEf(DataContext context)
    {
        this.context = context;
    }

    public async Task AddEmployee(Employee employee)
    {
        context.ChangeTracker.Clear();

        await context.Employees.AddAsync(employee);

        await context.SaveChangesAsync();
    }

    public async Task<Employee?> GetEmployee(Guid id)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Employee>> GetAllEmployees()
    {
        return await context.Employees
            .Select(e => e)
            .ToListAsync();
    }

    public async Task<List<Tuple<string?, string?>>> GetFilterNames()
    {
        return await context.Employees
            .Where(e=> e.DateOfBirth.Year > 1980 && e.Salary < 4000)
            .OrderBy(e=>e.FirstName)
            .Select(e => new Tuple<string?, string?>(e.FirstName, e.LastName))
            .ToListAsync();
    }

    public async Task UpdateEmployee(Employee employee)
    {
        context.ChangeTracker.Clear();

        context.Employees.Update(employee);

        await context.SaveChangesAsync();
    }

    public async Task DeleteEmployee(Employee employee)
    {
        context.Employees.Remove(employee);

        await context.SaveChangesAsync();
    }

    public async Task<Employee?> GetEmployeeAsNoTracking(Guid id)
    {
        return await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Employee>> GetAllEmployeesAsNoTracking()
    {
        return await context.Employees
            .AsNoTracking()
            .Select(e => e)
            .ToListAsync();
    }

    public async Task<Employee?> GetEmployeeWithRawSQL(Guid id)
    {
        return await context.Employees
            .FromSqlRaw("SELECT * FROM Employees WHERE Id = {0} ", id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Employee>> GetAllEmployeesWithRawSQL()
    {
        return await context.Employees
            .FromSqlRaw("SELECT * FROM Employees ")
            .ToListAsync();
    }

    public async Task<Employee?> GetEmployeeCompiled(Guid id)
    {
        return await FirstOrDefaultEmployeeAsync(context, id);
    }

    public async Task<List<Employee>> GetAllEmployeesCompiled()
    {
        var result = new List<Employee>();

        await foreach (var employee in AllEmployeesAsync(context))
        {
            result.Add(employee);
        }
        return result;
    }

    public async Task ExecuteUpdateEmployee(Employee employee)
    {
        await context.Employees
            .Where(e => e.Id == employee.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.FirstName, employee.FirstName)
                .SetProperty(e => e.LastName, employee.LastName)
                .SetProperty(e => e.Position, employee.Position)
                .SetProperty(e => e.Address, employee.Address)
                .SetProperty(e => e.DateOfBirth, employee.DateOfBirth)
                .SetProperty(e => e.Salary, employee.Salary));
    }

    public async Task ExecuteDeleteEmployee(Guid id)
    {
        await context.Employees
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    }
}