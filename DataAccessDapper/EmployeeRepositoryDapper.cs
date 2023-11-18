using Domain;
using Domain.Contracts;
using BenchmarkDotNet.Attributes;
using Dapper;
using DataAccessDapper.Interfaces;

namespace DataAccessDapper;

[MemoryDiagnoser]
public class EmployeeRepositoryDapper : IEmployeeRepository
{
    private readonly ISqlConnectionFactory sqlConnectionFactory;

    public EmployeeRepositoryDapper(ISqlConnectionFactory sqlConnectionFactory)
    {
        this.sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task AddEmployee(Employee employee)
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "INSERT INTO Employees (Id, FirstName, LastName, Position, Address, DateOfBirth, Salary)" +
            "VALUES (@Id, @FirstName, @LastName, @Position, @Address, @DateOfBirth, @Salary) ", employee);
    }

    public async Task<Employee?> GetEmployee(Guid id)
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        var employee = await connection.QuerySingleOrDefaultAsync<Employee?>(
            "SELECT * FROM Employees WHERE Id = @id ", new { id = id });

        return employee;
    }

    public async Task<List<Employee>> GetAllEmployees()
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        var employees = await connection.QueryAsync<Employee>(
            "SELECT * FROM Employees ");

        return employees.ToList();
    }

    public async Task<List<Tuple<string?, string?>>> GetFilterNames()
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        var employees = await connection.QueryAsync<(string?, string?)>(
            "SELECT [FirstName], [LastName] " +
            "FROM Employees " +
            "Where YEAR([DateOfBirth]) > 1980 and Salary < 4000" +
            "ORDER BY [FirstName]");

        return employees.Select(x => new Tuple<string?, string?>(x.Item1, x.Item2)).ToList();
    }

    public async Task UpdateEmployee(Employee employee)
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "UPDATE Employees " +
            "SET [FirstName] = @FirstName, " +
            "[LastName] = @LastName, " +
            "[Position] = @Position, " +
            "[Address] = @Address, " +
            "[DateOfBirth] = @DateOfBirth, " +
            "[Salary] = @Salary " +
            "WHERE Id = @Id", employee);
    }

    public async Task DeleteEmployee(Employee employee)
    {
        await using var connection = sqlConnectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "DELETE FROM Employees WHERE Id = @Id", employee);
    }
}