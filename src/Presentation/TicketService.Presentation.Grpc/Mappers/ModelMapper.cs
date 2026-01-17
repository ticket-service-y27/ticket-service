using Google.Protobuf.WellKnownTypes;
using TicketService.Grpc.Tickets;
using Ticket = TicketService.Application.Models.Tickets.Ticket;
using TicketStatus = TicketService.Application.Models.Tickets.TicketStatus;

namespace TicketService.Presentation.Grpc.Mappers;

public class ModelMapper
{
    public TicketStatus Map(TicketService.Grpc.Tickets.TicketStatus status)
    {
        return status switch
        {
            TicketService.Grpc.Tickets.TicketStatus.Reserved => TicketStatus.Reserved,
            TicketService.Grpc.Tickets.TicketStatus.Paid => TicketStatus.Paid,
            TicketService.Grpc.Tickets.TicketStatus.Cancelled => TicketStatus.Cancelled,
            TicketService.Grpc.Tickets.TicketStatus.Refunded => TicketStatus.Refunded,
            TicketService.Grpc.Tickets.TicketStatus.Unspecified => TicketStatus.Cancelled,
            _ => TicketStatus.Cancelled,
        };
    }

    public TicketService.Grpc.Tickets.TicketStatus Map(TicketStatus status)
    {
        return status switch
        {
            TicketStatus.Reserved => TicketService.Grpc.Tickets.TicketStatus.Reserved,
            TicketStatus.Paid => TicketService.Grpc.Tickets.TicketStatus.Paid,
            TicketStatus.Cancelled => TicketService.Grpc.Tickets.TicketStatus.Cancelled,
            TicketStatus.Refunded => TicketService.Grpc.Tickets.TicketStatus.Refunded,
            _ => TicketService.Grpc.Tickets.TicketStatus.Unspecified,
        };
    }

    public CreateTicketResponse Map(long ticketId)
    {
        return new() { TicketId = ticketId };
    }

    public UpdateTicketStatusResponse Map(bool success)
    {
        return new() { Success = success };
    }

    public PayTicketResponse Map(bool success, string failReason)
    {
        return new()
        {
            Success = success,
            FailReason = failReason,
        };
    }

    public TicketService.Grpc.Tickets.Ticket Map(Ticket ticket)
        => new()
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            EventId = ticket.EventId,
            SeatRow = ticket.Row,
            SeatNumber = ticket.Number,
            Price = ticket.Price,
            PaymentId = ticket.PaymentId,
            Status = Map(ticket.Status),
            CreatedAt = Timestamp.FromDateTimeOffset(ticket.CreatedAt),
            AppliedPromocode = ticket.AppliedPromocode ?? string.Empty,
        };
}