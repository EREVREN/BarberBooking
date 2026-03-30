using BarberBooking.Application.Customers.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberBooking.Application.Customers.Queries
{
    public sealed record GetCustomerByPhoneQuery(
        string PhoneNumber

        ):IRequest<CustomerDto>;
    
}
