
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBooking.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration
    : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.StartTime)
               .IsRequired();

        builder.Property(a => a.EndTime)
               .IsRequired();

        builder.Property(a => a.Status)
               .HasConversion<int>()
               .IsRequired();

        builder.HasIndex(a => new { a.BarberId, a.StartTime });
        builder.HasIndex(a => new { a.BarberId, a.EndTime });
    }
}
