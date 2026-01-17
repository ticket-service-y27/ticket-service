using PaymentService.Grpc.Payments;
using TicketService.Application.Contracts.Clients;
using PaymentFailReason = TicketService.Application.Contracts.Clients.PaymentFailReason;

namespace TicketService.Presentation.Grpc.Clients;

public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly PaymentsService.PaymentsServiceClient _client;

    public PaymentServiceClient(PaymentsService.PaymentsServiceClient client)
    {
        _client = client;
    }

    public async Task<PayResult> TryPay(long paymentId, CancellationToken cancellationToken)
    {
        var req = new TryPayRequest()
        {
            PaymentId = paymentId,
        };

        TryPayResponse resp = await _client.TryPayAsync(req, cancellationToken: cancellationToken);

        return new PayResult(resp.Success, MapReason(resp.Reason));
    }

    public async Task<long> CreatePayment(long userId, long amount, CancellationToken cancellationToken)
    {
        var req = new CreatePaymentRequest
        {
            UserId = userId,
            Amount = amount,
        };

        CreatePaymentResponse resp = await _client.CreatePaymentAsync(req, cancellationToken: cancellationToken);

        return resp.PaymentId;
    }

    public async Task<bool> RefundMoney(long paymentId, CancellationToken cancellationToken)
    {
        var req = new TransferPaymentToRefundedRequest
        {
            PaymentId = paymentId,
        };

        TransferPaymentToRefundedResponse resp = await _client.TransferPaymentToRefundedAsync(req, cancellationToken: cancellationToken);

        return resp.IsSuccess;
    }

    private static PaymentFailReason? MapReason(
        PaymentService.Grpc.Payments.PaymentFailReason reason)
    {
        return reason switch
        {
            PaymentService.Grpc.Payments.PaymentFailReason.Unspecified => PaymentFailReason.InternalError,
            PaymentService.Grpc.Payments.PaymentFailReason.NotEnoughMoney => PaymentFailReason.NotEnoughMoney,
            PaymentService.Grpc.Payments.PaymentFailReason.UserIsBlocked => PaymentFailReason.UserIsBlocked,
            PaymentService.Grpc.Payments.PaymentFailReason.InternalError => PaymentFailReason.InternalError,
            PaymentService.Grpc.Payments.PaymentFailReason.PaymentNotFound => PaymentFailReason.PaymentNotFound,
            PaymentService.Grpc.Payments.PaymentFailReason.WalletNotFound => PaymentFailReason.WalletNotFound,
            _ => null,
        };
    }
}