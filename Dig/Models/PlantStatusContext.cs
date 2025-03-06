using Microsoft.EntityFrameworkCore;

namespace Dig.Models;

public class PlantStatusContext : DbContext
{
    public PlantStatusContext (DbContextOptions<PlantStatusContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<PlantStatus> PlantStatuses { get; set; } = null!;
}