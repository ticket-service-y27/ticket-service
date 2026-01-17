using TicketService.Application.Models.Promocodes;

namespace TicketService.Application.Abstractions.Promocodes;

public interface IPromocodeRepository
{
    Task CreatePromocodeAsync(
        string promo,
        long discountPercentage,
        long count,
        CancellationToken cancellationToken);

    Task<Promocode?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    Task UpdateAsync(string code, long count, CancellationToken cancellationToken);
}