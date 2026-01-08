
using Application.Interfaces.Repositories;
using BarberBooking.Application.Interfaces.Repositories;

using BarberBooking.Application.Models;
using BarberBooking.Domain.Entities;
namespace BarberBooking.Application.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IServiceRepository _services;
        private readonly ICustomerRepository _customers;

        public AppointmentService(
            IAppointmentRepository appointments,
            IServiceRepository services,
            ICustomerRepository customers)
        {
            _appointments = appointments;
            _services = services;
            _customers = customers;
        }

        public async Task<BookingResult> BookAsync(
            Guid barberId,
            Guid serviceId,
            string customerPhone,
            DateTime startTime)
        {
            var service = await _services.GetByIdAsync(serviceId)
                ?? throw new InvalidOperationException("Service not found");

            var endTime = startTime.AddMinutes(service.DurationMinutes);

            if (await _appointments.ExistsOverlapping(
                    barberId, startTime, endTime))
                throw new InvalidOperationException("Slot already booked");

            var customer = await _customers.GetByPhoneAsync(customerPhone)
                ?? new Customer("Guest", customerPhone);

            if (customer.Id == Guid.Empty)
                await _customers.AddAsync(customer);

            var appointment = new Appointment(
                barberId,
                customer.Id,
                serviceId,
                startTime,
                endTime);

            await _appointments.AddAsync(appointment);

            return new BookingResult
            {
                AppointmentId = appointment.Id,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString()
            };
        }
    }
}
