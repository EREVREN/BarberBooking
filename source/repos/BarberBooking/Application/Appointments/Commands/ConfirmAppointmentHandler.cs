using BarberBooking.Application.Common.Messaging;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Contracts.Events;
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Enums;
using BarberBooking.Domain.Exceptions;

using MediatR;


namespace BarberBooking.Application.Appointments.Commands;

public sealed class ConfirmAppointmentHandler
    : IRequestHandler<ConfirmAppointmentCommand, Guid>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventPublisher _eventPublisher;

    public ConfirmAppointmentHandler(IAppointmentRepository appointmentRepository,
        IEventPublisher eventPublisher,
        IServiceRepository serviceRepository,
        ICustomerRepository customerRepository)
    {
        _appointmentRepository = appointmentRepository;
        _eventPublisher = eventPublisher;
        _serviceRepository = serviceRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(
        ConfirmAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("🔥 CONFIRM HANDLER HIT");
        var hasConflict = await _appointmentRepository.ExistsOverlapping(
            request.BarberId,
            request.StartTime,
            request.EndTime);

        if (hasConflict)
            throw new AppointmentConflictException();


        var appointment = Appointment.Create(
            request.BarberId,
            request.CustomerId,
            request.ServiceId,
            request.StartTime,
            request.EndTime,
            AppointmentStatus.Confirmed
        );

        await _appointmentRepository.AddAsync(appointment);

        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        var serviceName = service != null ? service.Name : "Unknown Service";
        var customerEmail = customer != null ? customer.Email : "Unknown Email";
        var customerPhone = customer != null ? customer.PhoneNumber : "Unknown Phone";

        var bookingEvent = new BookingConfirmedEvent(
            appointment.Id,
            appointment.BarberId,
            appointment.CustomerId,
            customerEmail,
            customerPhone,
            serviceName,
            appointment.StartTime,
            appointment.EndTime,
            DateTime.UtcNow
        );

        await _eventPublisher.PublishAsync(
                "booking.confirmed",
            bookingEvent,
            cancellationToken);
       

        return appointment.Id;
    }
}
