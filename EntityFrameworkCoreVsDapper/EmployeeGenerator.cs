using Bogus;
using DataAccessEFCore;
using Domain;

namespace EntityFrameworkCoreVsDapperApp;

public class EmployeeGenerator
{
    private readonly DataContext dataContext;

    private List<Employee> employees = new();

    private readonly Faker<Employee> employeeGenerator = new Faker<Employee>()
        .RuleFor(e => e.Id, f => f.Random.Guid())
        .RuleFor(e => e.FirstName, f => f.Name.FirstName())
        .RuleFor(e => e.LastName, f => f.Name.LastName())
        .RuleFor(e => e.Address, f => f.Address.Locale)
        .RuleFor(e => e.Position, f => f.Name.JobTitle())
        .RuleFor(e => e.DateOfBirth, f => f.Person.DateOfBirth)
        .RuleFor(e => e.Salary, f => f.Random.Double(1200, 3000));

    public EmployeeGenerator(Random random, DataContext dataContext)
    {
        this.dataContext = dataContext;
        Randomizer.Seed = random;
    }

    public async Task GenerateEmployees(int count)
    {
        employees = employeeGenerator.Generate(count);

        await dataContext.AddRangeAsync(employees);
        await dataContext.SaveChangesAsync();

        await dataContext.Employees.AddAsync(SharedEmployee.GetEmployee());
        await dataContext.SaveChangesAsync();

        dataContext.ChangeTracker.Clear();
    }

    public async Task CleanupEmployees()
    {
        dataContext.ChangeTracker.Clear();

        dataContext.Employees.RemoveRange(employees);
        await dataContext.SaveChangesAsync();

        dataContext.Employees.Remove(SharedEmployee.GetEmployee());
        await dataContext.SaveChangesAsync();
    }
}

