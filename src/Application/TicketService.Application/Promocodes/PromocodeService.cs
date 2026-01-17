using System.Transactions;
using TicketService.Application.Abstractions.Promocodes;
using TicketService.Application.Contracts.Promocodes;
using TicketService.Application.Models.Promocodes;

namespace TicketService.Application.Promocodes;

public class PromocodeService : IPromocodesService
{
    private readonly IPromocodeRepository _promocodeRepository;

    public PromocodeService(IPromocodeRepository promocodeRepository)
    {
        _promocodeRepository = promocodeRepository;
    }

    public async Task CreatePromocodeAsync(string promo, long discountPercentage, long count, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _promocodeRepository.CreatePromocodeAsync(promo, discountPercentage, count, cancellationToken);
        scope.Complete();
    }

    public async Task<Promocode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        Promocode? promocode = await _promocodeRepository.GetByCodeAsync(code, cancellationToken);

        if (promocode == null)
        {
            throw new PromocodeException("promocode not found");
        }

        return promocode;
    }
}