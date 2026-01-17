namespace TicketService.Application.Models.Promocodes;

public class PromocodeException : Exception
{
    public PromocodeException(string message) : base(message) { }
}