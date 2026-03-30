using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBooking.Infrastructure.Persistence.Configurations;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Many-to-Many relationship configuration via Join Entity
        builder.HasMany(b => b.BarberServices)
               .WithOne(bs => bs.Barber)
               .HasForeignKey(bs => bs.BarberId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Barber.BarberServices))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        
    }
}
