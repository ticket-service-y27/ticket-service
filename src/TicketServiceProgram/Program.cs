using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TicketService.Application.Extensions;
using TicketService.Infrastructure.DataAccess.Extensions;
using TicketService.Presentation.Grpc.Extensions;
using TicketService.Presentation.Grpc.Interceptors;
using TicketService.Presentation.Grpc.Mappers;
using TicketService.Presentation.Grpc.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ErrorHandling>();
});

builder.Services
    .AddDatabaseOptions(builder.Configuration)
    .AddMigrations()
    .AddNpgsqlDataSource()
    .AddMigrationHostedService()
    .AddRepositories()
    .AddApplication()
    .AddSingleton<ModelMapper>()
    .AddGrpcPaymentServiceClient(builder.Configuration)
    .AddGrpcEventServiceClient(builder.Configuration);

WebApplication app = builder.Build();

app.MapGrpcService<TicketGrpcService>();
app.MapGrpcService<PromocodeGrpcService>();

app.Run();