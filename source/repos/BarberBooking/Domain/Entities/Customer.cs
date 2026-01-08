namespace BarberBooking.Domain.Entities
{
    public class Customer : Common.BaseEntity
    {
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }

        protected Customer() { }

        public Customer(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }
}
