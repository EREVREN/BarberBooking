using BarberBooking.Application.BlockedSlots.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;


namespace BarberBooking.Application.BlockedSlots.Queries
{
    public class GetBlockedSlotHandler : IRequestHandler<GetBlockedSlotQuery, List<BlockedSlotDto>>
    {
        public readonly IBlockedSlotRepository _blockedSlotRepository;
        public GetBlockedSlotHandler(IBlockedSlotRepository blockedSlotRepository)
        {
            _blockedSlotRepository = blockedSlotRepository;
        }

        public async Task<List<BlockedSlotDto>> Handle(GetBlockedSlotQuery request, CancellationToken cancellationToken)
        {
            var blockedSlots = await _blockedSlotRepository.GetForBarberAndDate(request.BarberId, request.Date);

            if (blockedSlots != null && blockedSlots.Count > 0)
            {
                var result = blockedSlots.Select(bs => new BlockedSlotDto(
                    bs.BarberId,
                    bs.StartTime,
                    bs.EndTime
                )).ToList();

                return result;
            }

            return new List<BlockedSlotDto>();
        }
    }
}

