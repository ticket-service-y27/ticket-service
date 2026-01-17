using TicketService.Application.Models.Tickets;

namespace TicketService.Application.Abstractions.Tickets;

public interface ITicketRepository
{
    Task<long> CreateAsync(
        long userId,
        long eventId,
        long price,
        long row,
        long number,
        long paymentId,
        TicketStatus status,
        CancellationToken cancellationToken,
        string? appliedPromocode);

    Task<Ticket?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task UpdateStatusAsync(long id, TicketStatus status, CancellationToken cancellationToken);

    IAsyncEnumerable<Ticket> GetAllTicketsByUserIdAsync(CancellationToken cancellationToken);
}