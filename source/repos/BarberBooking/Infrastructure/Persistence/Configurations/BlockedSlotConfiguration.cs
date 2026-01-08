using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBooking.InfraStructure.Persistence.Configurations
{
    public class BlockedSlotConfiguration : IEntityTypeConfiguration<BlockedSlot>
    {
        public void Configure(EntityTypeBuilder<BlockedSlot> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasIndex(b => new { b.BarberId, b.StartTime });

            builder.Property(b => b.StartTime).IsRequired();
            builder.Property(b => b.EndTime).IsRequired();
        }
    }
}
