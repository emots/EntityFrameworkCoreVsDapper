using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEFCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) 
        : base(options)
    { }

    public DbSet<Employee> Employees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Id);
    }
}