using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BarberBooking.InfraStructure.Persistence.Configurations
{
    public class BarberWorkingDayConfiguration : IEntityTypeConfiguration<BarberWorkingDay>
    {
        public void Configure(EntityTypeBuilder<BarberWorkingDay> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasIndex(w => new { w.BarberId, w.DayOfWeek })
                   .IsUnique();

            builder.Property(w => w.DayOfWeek)
                   .HasConversion<int>();

            builder.Property(w => w.StartTime).IsRequired();
            builder.Property(w => w.EndTime).IsRequired();
        }
    }
}
