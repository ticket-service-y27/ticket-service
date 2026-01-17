using EventService.Presentation.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentService.Grpc.Payments;
using TicketService.Application.Contracts.Clients;
using TicketService.Presentation.Grpc.Clients;
using TicketService.Presentation.Grpc.Options;

namespace TicketService.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcPaymentServiceClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PaymentServiceOptions>()
            .Bind(configuration.GetSection("UserService"));

        services.AddGrpcClient<PaymentsService.PaymentsServiceClient>((sp, options) =>
        {
            PaymentServiceOptions cfg = sp.GetRequiredService<IOptions<PaymentServiceOptions>>().Value;
            options.Address = new Uri(cfg.Url);
        });

        services.AddScoped<IPaymentServiceClient, PaymentServiceClient>();

        return services;
    }

    public static IServiceCollection AddGrpcEventServiceClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<EventServiceOptions>()
            .Bind(configuration.GetSection("EventService"));

        services.AddGrpcClient<SeatValidationGrpcService.SeatValidationGrpcServiceClient>((sp, options) =>
        {
            EventServiceOptions cfg = sp.GetRequiredService<IOptions<EventServiceOptions>>().Value;
            options.Address = new Uri(cfg.Url);
        });

        services.AddScoped<IEventServiceClient, EventsServiceClient>();

        return services;
    }
}