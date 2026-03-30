using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using MediatR;
namespace BarberBooking.Application.BlockedSlots.Commands
{
    internal class CreateBlockedSlotHandler : IRequestHandler<CreateBlockedSlotCommand, Guid>
    {
       

        public readonly IBlockedSlotRepository _blockedSlotRepository;

        public CreateBlockedSlotHandler(IBlockedSlotRepository blockedSlotRepository)
        {
            _blockedSlotRepository = blockedSlotRepository;
            
        }

        public async Task<Guid> Handle(CreateBlockedSlotCommand request, CancellationToken cancellationToken)
        {
            var blockedSlot = new BlockedSlot (
            
                request.BarberId,
                request.StartTime,
                request.EndTime
                
            );

            if (blockedSlot.StartTime >= blockedSlot.EndTime)
            {
                throw new ArgumentException("Start time must be before end time.");
            }
            await _blockedSlotRepository.AddAsync(blockedSlot);
            return blockedSlot.Id;
        }
    }
}
