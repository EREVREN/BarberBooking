using MediatR;

namespace BarberBooking.Application.Common.Pagination;

public abstract record PagedQuery<TResponse>(
    int Page = 1,
    int PageSize = 20
) : IRequest<TResponse>
{
    public int Skip => (Page - 1) * PageSize;
}
