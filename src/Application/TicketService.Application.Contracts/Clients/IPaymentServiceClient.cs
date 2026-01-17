namespace TicketService.Application.Contracts.Clients;

public interface IPaymentServiceClient
{
    Task<PayResult> TryPay(long paymentId, CancellationToken cancellationToken);

    Task<long> CreatePayment(long userId, long amount, CancellationToken cancellationToken);

    Task<bool> RefundMoney(long paymentId, CancellationToken cancellationToken);
}