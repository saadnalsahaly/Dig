using Dig.Models;
using Microsoft.EntityFrameworkCore;

namespace Dig.Data;

public class UserCommandContext : DbContext
{
    public UserCommandContext(DbContextOptions<UserCommandContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<UserCommand> UserCommands { get; set; } = null!;
}