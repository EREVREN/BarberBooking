using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.HasKey(b => b.Id);

        // 🔴 IMPORTANT: Ignore the property navigation
        builder.Ignore(b => b.Services);

        // ✅ Map field-only navigation
        builder.HasMany<Service>("_services")
               .WithOne()
               .HasForeignKey("BarberId")
               .OnDelete(DeleteBehavior.Cascade);
    }
}