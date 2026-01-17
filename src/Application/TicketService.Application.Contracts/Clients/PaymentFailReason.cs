namespace TicketService.Application.Contracts.Clients;

public enum PaymentFailReason
{
    NotEnoughMoney,
    UserIsBlocked,
    InternalError,
    PaymentNotFound,
    WalletNotFound,
}