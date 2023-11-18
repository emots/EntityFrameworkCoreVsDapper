using BenchmarkDotNet.Attributes;
using DataAccessDapper;
using DataAccessDapper.Interfaces;
using DataAccessEFCore;
using DataAccessEFCore.Interfaces;
using Domain;
using Domain.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;

namespace EntityFrameworkCoreVsDapperApp;

[MemoryDiagnoser]
public class Benchmarks
{
    private const string ConnectionStringName = "EmployeeDb";

    private readonly Employee employeeForAddDelete = new()
    {
        Id = new Guid("f88b2138-1b7d-485b-862d-85ea629cf31c"),
        FirstName = "FirstName",
        LastName = "LastName",
        Position = "Position",
        Address = "Address",
        DateOfBirth = DateTime.UtcNow,
        Salary = 1000
    };

    private IHost host = null!;
    private IEmployeeRepository employeeRepositoryEf = null!;
    private IEmployeeRepositoryEfExtend employeeRepositoryEfExtend = null!;
    private IEmployeeRepository employeeRepositoryDapper = null!;

    private Random random = null!;
    private DataContext context = null!;
    private EmployeeGenerator employeeGenerator = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        var builder = Host.CreateApplicationBuilder();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddSqlServer<DataContext>(configuration.GetConnectionString(ConnectionStringName));
        builder.Services.AddOptions<DatabaseSettings>().Bind(configuration.GetSection("ConnectionStrings"));
        builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        builder.Services.AddScoped<EmployeeRepositoryEf>();
        builder.Services.AddScoped<EmployeeRepositoryDapper>();

        host = builder.Build();

        context = host.Services.GetService<DataContext>()!;
        employeeRepositoryEf = host.Services.GetService<EmployeeRepositoryEf>()!;
        employeeRepositoryEfExtend = host.Services.GetService<EmployeeRepositoryEf>()!;
        employeeRepositoryDapper = host.Services.GetService<EmployeeRepositoryDapper>()!;

        random = new Random(1100);

        employeeGenerator = new EmployeeGenerator(random, context);

        await employeeGenerator.GenerateEmployees(1000);
    }

    [Benchmark]
    public async Task Get_One_EFCore()
    {
        var result = await employeeRepositoryEf.GetEmployee(SharedEmployee.Id);
    }

    [Benchmark]
    public async Task Get_One_EFCore_AsNoTracking()
    {
        var result = await employeeRepositoryEfExtend.GetEmployeeAsNoTracking(SharedEmployee.Id);
    }

    [Benchmark]
    public async Task Get_One_EFCore_RawSQL()
    {
        var result = await employeeRepositoryEfExtend.GetEmployeeWithRawSQL(SharedEmployee.Id);
    }

    [Benchmark]
    public async Task Get_One_EFCore_Complied()
    {
        var result = await employeeRepositoryEfExtend.GetEmployeeCompiled(SharedEmployee.Id);
    }

    [Benchmark]
    public async Task Get_One_Dapper()
    {
        var result = await employeeRepositoryDapper.GetEmployee(SharedEmployee.Id);
    }

    [Benchmark]
    public async Task Get_All_EFCore()
    {
        await employeeRepositoryEf.GetAllEmployees();
    }

    [Benchmark]
    public async Task Get_All_EFCore_AsNoTracking()
    {
        var result = await employeeRepositoryEfExtend.GetAllEmployeesAsNoTracking();
    }

    [Benchmark]
    public async Task Get_All_EFCore_RawSQL()
    {
        var result = await employeeRepositoryEfExtend.GetAllEmployeesWithRawSQL();
    }

    [Benchmark]
    public async Task Get_All_EFCore_Complied()
    {
        var result = await employeeRepositoryEfExtend.GetAllEmployeesCompiled();
    }

    [Benchmark]
    public async Task Get_All_Dapper()
    {
        await employeeRepositoryDapper.GetAllEmployees();
    }

    [Benchmark]
    public async Task Get_Filter_EFCore()
    {
        await employeeRepositoryEf.GetFilterNames();
    }

    [Benchmark]
    public async Task Get_Filter_Dapper()
    {
        await employeeRepositoryDapper.GetFilterNames();
    }

    [Benchmark]
    public async Task Update_EFCore()
    {
        var employee = SharedEmployee.GetEmployee();

        employee.FirstName = "NewFirstName";
        employee.LastName = "NewLastName";
        employee.Position = "NewPosition";
        employee.Address = "NewAddress";
        employee.DateOfBirth = DateTime.UtcNow;
        employee.Salary = 1111;

        await employeeRepositoryEf.UpdateEmployee(employee);
    }

    [Benchmark]
    public async Task Execute_Update_EFCore()
    {
        var employee = SharedEmployee.GetEmployee();

        employee.FirstName = "NewFirstName";
        employee.LastName = "NewLastName";
        employee.Position = "NewPosition";
        employee.Address = "NewAddress";
        employee.DateOfBirth = DateTime.UtcNow;
        employee.Salary = 1111;

        await employeeRepositoryEfExtend.ExecuteUpdateEmployee(employee);
    }

    [Benchmark]
    public async Task Update_Dapper()
    {
        var employee = SharedEmployee.GetEmployee();

        employee.FirstName = "NewFirstName";
        employee.LastName = "NewLastName";
        employee.Position = "NewPosition";
        employee.Address = "NewAddress";
        employee.DateOfBirth = DateTime.UtcNow;
        employee.Salary = 1111;

        await employeeRepositoryDapper.UpdateEmployee(employee);
    }

    [Benchmark]
    public async Task Add_Delete_EFCore()
    {
        await employeeRepositoryEf.AddEmployee(employeeForAddDelete);
        await employeeRepositoryEf.DeleteEmployee(employeeForAddDelete);
    }

    [Benchmark]
    public async Task Add_Execute_Delete_EFCore()
    {
        await employeeRepositoryEf.AddEmployee(employeeForAddDelete);
        await employeeRepositoryEfExtend.ExecuteDeleteEmployee(employeeForAddDelete.Id);
    }

    [Benchmark]
    public async Task Add_Delete_Dapper()
    {
        await employeeRepositoryDapper.AddEmployee(employeeForAddDelete);
        await employeeRepositoryDapper.DeleteEmployee(employeeForAddDelete);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await employeeGenerator.CleanupEmployees();

        host.Dispose();
    }
}