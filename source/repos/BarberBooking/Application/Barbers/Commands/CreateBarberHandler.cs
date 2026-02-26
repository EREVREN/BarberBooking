using MediatR;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Application.Barbers.Commands;
using BarberBooking.Domain.Entities;




public class CreateBarberHandler : IRequestHandler<CreateBarberCommand, Guid>
{
    private readonly IBarberRepository _repository;

    public CreateBarberHandler(IBarberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateBarberCommand request,
        CancellationToken cancellationToken)
    {
        var barber = new Barber(request.Name);
        await _repository.AddAsync(barber);
        return barber.Id;
    }
}
