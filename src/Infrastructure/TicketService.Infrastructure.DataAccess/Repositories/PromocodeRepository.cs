using Npgsql;
using TicketService.Application.Abstractions.Promocodes;
using TicketService.Application.Models.Promocodes;

namespace TicketService.Infrastructure.DataAccess.Repositories;

public class PromocodeRepository : IPromocodeRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PromocodeRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task CreatePromocodeAsync(
        string promo,
        long discountPercentage,
        long count,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into promocodes (code, discount_percentage, count)
                           values (@Code, @DiscountPercentage, @Count)
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@Code", promo));
        command.Parameters.Add(new NpgsqlParameter("@DiscountPercentage", discountPercentage));
        command.Parameters.Add(new NpgsqlParameter("@Count", count));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Promocode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        const string sql = """
                           select id, code, discount_percentage, count
                           from promocodes
                           where code = @Code
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@Code", code));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return new Promocode(
            Id: reader.GetInt64(0),
            Promo: reader.GetString(1),
            DiscountPercentage: reader.GetInt64(2),
            Count: reader.GetInt64(3));
    }

    public async Task UpdateAsync(string code, long count, CancellationToken cancellationToken)
    {
        const string sql = """
                           update promocodes
                           set count = @Count
                           where code = @Code
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@Code", code));
        command.Parameters.Add(new NpgsqlParameter("@Count", count));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}