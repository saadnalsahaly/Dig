using Microsoft.EntityFrameworkCore;
using Environment = Dig.Models.Environment;

namespace Dig.Data;

public class EnvironmentContext : DbContext
{
    public EnvironmentContext(DbContextOptions<EnvironmentContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Environment> Environments { get; set; } = null!;
}