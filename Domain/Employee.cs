namespace Domain;

public class Employee
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public string? Address { get; set; }

    public DateTime DateOfBirth { get; set; }

    public double Salary { get; set; }
}