namespace BarberBooking.Domain.ValueObjects
{
    public class WorkingHours
    {
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }

        protected WorkingHours() { }

        public WorkingHours(TimeSpan start, TimeSpan end)
        {
            if (start >= end)
                throw new ArgumentException("Start must be before End");

            Start = start;
            End = end;
        }
    }
}
