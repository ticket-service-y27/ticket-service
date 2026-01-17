using TicketService.Application.Models.Promocodes;

namespace TicketService.Application.Contracts.Promocodes;

public interface IPromocodesService
{
    Task CreatePromocodeAsync(
        string promo,
        long discountPercentage,
        long count,
        CancellationToken cancellationToken);

    Task<Promocode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}