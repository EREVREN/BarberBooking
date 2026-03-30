using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBooking.Infrastructure.Persistence.Configurations;

public class BarberServiceConfiguration : IEntityTypeConfiguration<BarberService>
{
    public void Configure(EntityTypeBuilder<BarberService> builder)
    {
        // Composite Key to ensure uniqueness and efficient lookup
        builder.HasKey(bs => new { bs.ServiceId, bs.BarberId });

        builder.HasOne(bs => bs.Barber)
               .WithMany(b => b.BarberServices)
               .HasForeignKey(bs => bs.BarberId);

        builder.HasOne(bs => bs.Service)
               .WithMany(s => s.BarberServices)
               .HasForeignKey(bs => bs.ServiceId);
    }
}
