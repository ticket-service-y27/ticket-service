namespace TicketService.Application.Models.Promocodes;

public record Promocode(long Id, string Promo, long DiscountPercentage, long Count);