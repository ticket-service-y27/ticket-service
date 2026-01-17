using System.Transactions;
using TicketService.Application.Abstractions.Promocodes;
using TicketService.Application.Abstractions.Tickets;
using TicketService.Application.Contracts.Clients;
using TicketService.Application.Contracts.Tickets;
using TicketService.Application.Models.Promocodes;
using TicketService.Application.Models.Tickets;

namespace TicketService.Application.Tickets;

public class TicketsService : ITicketService
{
    private readonly ITicketRepository _repository;
    private readonly IPromocodeRepository _promocodeRepository;
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly IEventServiceClient _eventServiceClient;

    public TicketsService(
        ITicketRepository repository,
        IPromocodeRepository promocodeRepository,
        IPaymentServiceClient paymentServiceClient,
        IEventServiceClient eventServiceClient)
    {
        _repository = repository;
        _promocodeRepository = promocodeRepository;
        _paymentServiceClient = paymentServiceClient;
        _eventServiceClient = eventServiceClient;
    }

    public async Task<long> CreateAsync(
        long userId,
        long eventId,
        long price,
        long row,
        long number,
        CancellationToken cancellationToken,
        string? appliedPromocode)
    {
        Promocode? promo = null;

        if (!string.IsNullOrEmpty(appliedPromocode))
        {
            promo = await _promocodeRepository.GetByCodeAsync(appliedPromocode, cancellationToken);
        }

        long paymentId = await _paymentServiceClient.CreatePayment(userId, price, cancellationToken);

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        if (promo != null)
        {
            price -= price * promo.DiscountPercentage / 100;
            long newCount = promo.Count - 1;
            await _promocodeRepository.UpdateAsync(promo.Promo, newCount, cancellationToken);
        }

        long id = await _repository.CreateAsync(
            userId,
            eventId,
            price,
            row,
            number,
            paymentId,
            TicketStatus.Reserved,
            cancellationToken,
            promo?.Promo);

        scope.Complete();

        return id;
    }

    public async Task<bool> PayTicketAsync(long id, CancellationToken cancellationToken)
    {
        Ticket? ticket = await _repository.GetByIdAsync(id, cancellationToken);

        if (ticket == null)
        {
            throw new TicketException("ticket not found");
        }

        PayResult resp = await _paymentServiceClient.TryPay(ticket.PaymentId, cancellationToken);

        if (!resp.Success)
        {
            await UpdateStatusAsync(ticket.Id, TicketStatus.Cancelled, cancellationToken);
            return false;
        }

        await UpdateStatusAsync(ticket.Id, TicketStatus.Paid, cancellationToken);
        return true;
    }

    public async Task<IAsyncEnumerable<Ticket>> GetAllTicketsByUserIdAsync(CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Ticket> tickets = _repository.GetAllTicketsByUserIdAsync(cancellationToken);

        if (await tickets.CountAsync(cancellationToken) == 0)
        {
            throw new TicketException("tickets not found");
        }

        return tickets;
    }

    public async Task<bool> RefundSeats(long id, long schemeId, CancellationToken cancellationToken)
    {
        Ticket? ticket = await _repository.GetByIdAsync(id, cancellationToken);

        if (ticket == null)
        {
            throw new TicketException("ticket not found");
        }

        RefundResult resp = await _eventServiceClient.RefundSeats(schemeId, ticket.Row, ticket.Number, cancellationToken);

        if (!resp.Success)
        {
            return false;
        }

        await UpdateStatusAsync(id, TicketStatus.Refunded, cancellationToken);
        bool res = await _paymentServiceClient.RefundMoney(ticket.PaymentId, cancellationToken);

        if (!string.IsNullOrEmpty(ticket.AppliedPromocode))
        {
            Promocode? promocode = await _promocodeRepository.GetByCodeAsync(ticket.AppliedPromocode, cancellationToken);
            if (promocode != null)
            {
                long newCount = promocode.Count + 1;
                await _promocodeRepository.UpdateAsync(ticket.AppliedPromocode, newCount, cancellationToken);
            }
        }

        if (!res)
        {
            return false;
        }

        return true;
    }

    private async Task UpdateStatusAsync(long id, TicketStatus status, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _repository.UpdateStatusAsync(id, status, cancellationToken);

        scope.Complete();
    }
}