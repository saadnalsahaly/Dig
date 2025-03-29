using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dig;

namespace Dig.Models
{
    public class PlantContext : DbContext
    {
        public PlantContext (DbContextOptions<PlantContext> options)
            : base(options)
        {
        }

        public DbSet<Plant> Plant { get; set; } = default!;
    }
}
