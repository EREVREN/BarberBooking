

namespace BarberBooking.Application.Customers.DTOs
{
    public sealed record CustomerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string? Email,
        string? Address
    );
            
        
   
}
