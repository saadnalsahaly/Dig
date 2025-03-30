using Dig.Models;
using Microsoft.EntityFrameworkCore;

namespace Dig.Data;

public class PlantStatusContext : DbContext
{
    public PlantStatusContext (DbContextOptions<PlantStatusContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<PlantStatus> PlantStatuses { get; set; } = null!;
}