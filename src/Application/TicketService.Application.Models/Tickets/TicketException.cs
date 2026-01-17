namespace TicketService.Application.Models.Tickets;

public class TicketException : Exception
{
    public TicketException(string message) : base(message) { }
}