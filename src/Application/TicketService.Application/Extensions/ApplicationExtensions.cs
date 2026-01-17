using Microsoft.Extensions.DependencyInjection;
using TicketService.Application.Contracts.Promocodes;
using TicketService.Application.Contracts.Tickets;
using TicketService.Application.Promocodes;
using TicketService.Application.Tickets;

namespace TicketService.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITicketService, TicketsService>();
        services.AddScoped<IPromocodesService, PromocodeService>();

        return services;
    }
}