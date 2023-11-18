using Domain;

namespace EntityFrameworkCoreVsDapperApp;

public static class SharedEmployee
{
    public static readonly Guid Id = new("eb2a337e-7a1d-4495-aa95-2d54d2c6c65a");

    private static Employee Employee => new()
    {
        Id = Id,
        FirstName = "FirstName",
        LastName = "LastName",
        Position = "Position",
        Address = "Address",
        DateOfBirth = DateTime.UtcNow,
        Salary = 1000
    };

    public static Employee GetEmployee() => Employee;
}