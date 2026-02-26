namespace BarberBooking.Domain.Entities
{
    public class BlockedSlot : Common.BaseEntity
    {
        public Guid BarberId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

       
        public BlockedSlot(Guid barberId, DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("Invalid blocked slot");

            BarberId = barberId;
            StartTime = startTime;
            EndTime = endTime;
        }

        protected BlockedSlot() { }

    }
}
