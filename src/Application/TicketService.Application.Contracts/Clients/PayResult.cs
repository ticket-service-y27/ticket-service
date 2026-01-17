namespace TicketService.Application.Contracts.Clients;

public record PayResult(bool Success, PaymentFailReason? FailReason);