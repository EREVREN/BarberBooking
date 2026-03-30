namespace BarberBooking.Domain.Entities
{
    public class Customer : Common.BaseEntity
    {
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        
       

        protected Customer()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
        }

        public Customer(string name, string phoneNumber, string? email, string? address)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
        }
    }
}
