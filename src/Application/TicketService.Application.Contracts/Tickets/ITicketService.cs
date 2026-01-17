using TicketService.Application.Models.Tickets;

namespace TicketService.Application.Contracts.Tickets;

public interface ITicketService
{
    Task<long> CreateAsync(
        long userId,
        long eventId,
        long price,
        long row,
        long number,
        CancellationToken cancellationToken,
        string? appliedPromocode);

    Task<bool> PayTicketAsync(long id, CancellationToken cancellationToken);

    Task<IAsyncEnumerable<Ticket>> GetAllTicketsByUserIdAsync(CancellationToken cancellationToken);

    Task<bool> RefundSeats(long id, long schemeId, CancellationToken cancellationToken);
}