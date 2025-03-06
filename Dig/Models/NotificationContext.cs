using Microsoft.EntityFrameworkCore;

namespace Dig.Models;

public class NotificationContext : DbContext
{
    public NotificationContext (DbContextOptions<NotificationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Notification> Notifications { get; set; } = null!;
}