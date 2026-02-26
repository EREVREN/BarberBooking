
using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class BarberWorkingDayConfig : IEntityTypeConfiguration<BarberWorkingDay>
{
    public void Configure(EntityTypeBuilder<BarberWorkingDay> builder)
    {
        builder.ToTable("BarberWorkingDays");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired(false);

        builder.OwnsOne(x => x.WorkingHours, wh =>
        {
            wh.Property(p => p.Start)
                .HasColumnName("StartTime")
                .IsRequired();

            wh.Property(p => p.End)
                .HasColumnName("EndTime")
                .IsRequired();
        });

        builder.HasIndex(x => new { x.BarberId, x.DayOfWeek });
        builder.HasIndex(x => new { x.BarberId, x.Date });
    }
}