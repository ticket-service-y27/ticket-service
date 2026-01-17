using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using TicketService.Application.Abstractions.Promocodes;
using TicketService.Application.Abstractions.Tickets;
using TicketService.Application.Models.Tickets;
using TicketService.Infrastructure.DataAccess.Migrations;
using TicketService.Infrastructure.DataAccess.Options;
using TicketService.Infrastructure.DataAccess.Repositories;
using TicketService.Infrastructure.DataAccess.Services;

namespace TicketService.Infrastructure.DataAccess.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddDatabaseOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection("DatabaseSettings"));

        return services;
    }

    public static IServiceCollection AddMigrationHostedService(
        this IServiceCollection services)
    {
        services.AddHostedService<MigrationHostedService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPromocodeRepository, PromocodeRepository>();
        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(sp =>
                    sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>().CurrentValue.ConnectionString)
                .ScanIn(typeof(CreateTicketsTable).Assembly).For.Migrations());

        return services;
    }

    public static IServiceCollection AddNpgsqlDataSource(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                string dbSettings = sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>().CurrentValue.ConnectionString;
                var sourceBuilder = new NpgsqlDataSourceBuilder(dbSettings);

                sourceBuilder.MapEnum<TicketStatus>();

                return sourceBuilder.Build();
            });

        return services;
    }
}