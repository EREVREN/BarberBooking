namespace BarberBooking.Domain.Entities
{
    public class Service : Common.BaseEntity
    {
        public string Name { get; private set; }
        public int DurationMinutes { get; private set; }
        public decimal Price { get; private set; }

        protected Service() { }

        public Service(string name, int durationMinutes, decimal price)
        {
            Name = name;
            DurationMinutes = durationMinutes;
            Price = price;
        }
    }
}

