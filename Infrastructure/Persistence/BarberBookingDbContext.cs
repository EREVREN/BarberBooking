using Microsoft.EntityFrameworkCore;
using BarberBooking.Domain.Entities;

namespace BarberBooking.Infrastructure.Persistence;

public class BarberBookingDbContext : DbContext
{
    public BarberBookingDbContext(DbContextOptions<BarberBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<BarberService> BarberServices => Set<BarberService>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<BlockedSlot> BlockedSlots => Set<BlockedSlot>();
    public DbSet<BarberWorkingDay> BarberWorkingDays => Set<BarberWorkingDay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberBookingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
