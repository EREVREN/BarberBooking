using Microsoft.EntityFrameworkCore;
using NotificationService.Scheduling;

namespace NotificationService.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options) { }

    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();
    public DbSet<Reminder> Reminders => Set<Reminder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProcessedEvent>()
            .HasKey(x => x.EventId);
    }
}
