namespace BarberBooking.Domain.Entities
{
    public class Customer : Common.BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        
       

        protected Customer() { }

        public Customer(string firstName, string lastName, string phoneNumber, string email, string address)
        {
          
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
