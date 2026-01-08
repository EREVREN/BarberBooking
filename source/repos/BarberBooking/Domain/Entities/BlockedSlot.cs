namespace BarberBooking.Domain.Entities
{
    public class BlockedSlot : Common.BaseEntity
    {
        public Guid BarberId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        protected BlockedSlot() { }

        public BlockedSlot(Guid barberId, DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Invalid blocked slot");

            BarberId = barberId;
            StartTime = start;
            EndTime = end;
        }
    }
}
