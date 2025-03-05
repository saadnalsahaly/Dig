using Microsoft.EntityFrameworkCore;

namespace Dig.Models;

public class EnvironmentContext : DbContext
{
    public EnvironmentContext(DbContextOptions<EnvironmentContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<EnvironmentData> Environments { get; set; } = null!;
}