namespace TicketService.Application.Models.Tickets;

public record Ticket(
    long Id,
    long UserId,
    long EventId,
    long Price,
    long Row,
    long Number,
    long PaymentId,
    TicketStatus Status,
    DateTimeOffset CreatedAt,
    string? AppliedPromocode);