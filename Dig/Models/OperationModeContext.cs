using Microsoft.EntityFrameworkCore;

namespace Dig.Models;

public class OperationModeContext : DbContext
{
    public OperationModeContext (DbContextOptions<OperationModeContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<OperationMode> OperationModes { get; set; } = null!;
}