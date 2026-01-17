namespace TicketService.Application.Contracts.Clients;

public interface IEventServiceClient
{
    Task<RefundResult> RefundSeats(long schemeId, long row, long number, CancellationToken cancellationToken);
}