namespace BarberBooking.Domain.Entities
{
    public class BarberWorkingDay : Common.BaseEntity
    {
        public Guid BarberId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }

        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        protected BarberWorkingDay() { }

        public BarberWorkingDay(Guid barberId, DayOfWeek dayOfWeek,
                                TimeSpan start, TimeSpan end)
        {
            if (start >= end)
                throw new ArgumentException("Invalid working hours");

            BarberId = barberId;
            DayOfWeek = dayOfWeek;
            StartTime = start;
            EndTime = end;
        }
    }
}
