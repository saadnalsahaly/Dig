using Dig.Models;
using Microsoft.EntityFrameworkCore;

namespace Dig.Data
{
    public class PlantContext : DbContext
    {
        public PlantContext (DbContextOptions<PlantContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Plant> Plant { get; set; } = default!;
    }
}
