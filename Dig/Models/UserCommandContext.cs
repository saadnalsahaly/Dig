using Microsoft.EntityFrameworkCore;

namespace Dig.Models;

public class UserCommandContext : DbContext
{
    public UserCommandContext(DbContextOptions<UserCommandContext> options)
        : base(options)
    {
        
    }

    public DbSet<UserCommand> UserCommands { get; set; } = null!;
}