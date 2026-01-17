using Npgsql;
using System.Runtime.CompilerServices;
using TicketService.Application.Abstractions.Tickets;
using TicketService.Application.Models.Tickets;

namespace TicketService.Infrastructure.DataAccess.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public TicketRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> CreateAsync(
        long userId,
        long eventId,
        long price,
        long row,
        long number,
        long paymentId,
        TicketStatus status,
        CancellationToken cancellationToken,
        string? appliedPromocode)
    {
        const string sql = """
                           insert into tickets (user_id, event_id, price, row, number, status, payment_id ,created_at, applied_promocode)
                           values (@UserId, @EventId, @Price, @Row, @Number, @Status, @PaymentId, @CreatedAt, @AppliedPromocode)
                           returning id
                           """;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@UserId", userId));
        command.Parameters.Add(new NpgsqlParameter("@EventId", eventId));
        command.Parameters.Add(new NpgsqlParameter("@Price", price));
        command.Parameters.Add(new NpgsqlParameter("@Row", row));
        command.Parameters.Add(new NpgsqlParameter("@Number", number));
        command.Parameters.Add(new NpgsqlParameter("@PaymentId", paymentId));

        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@Status",
            Value = status,
            DataTypeName = "ticket_status",
        });

        command.Parameters.Add(new NpgsqlParameter("@CreatedAt", now));

        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@AppliedPromocode",
            Value = (object?)appliedPromocode ?? DBNull.Value,
        });

        return (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
    }

    public async Task<Ticket?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           select id, user_id, event_id, price, row, number ,status, payment_id, created_at, applied_promocode
                           from tickets
                           where id = @Id
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@Id", id));

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return new Ticket(
            Id: reader.GetInt64(0),
            UserId: reader.GetInt64(1),
            EventId: reader.GetInt64(2),
            Price: reader.GetInt64(3),
            Row: reader.GetInt64(4),
            Number: reader.GetInt64(5),
            Status: reader.GetFieldValue<TicketStatus>(6),
            PaymentId: reader.GetInt64(7),
            CreatedAt: reader.GetFieldValue<DateTimeOffset>(8),
            AppliedPromocode: reader.IsDBNull(9) ? null : reader.GetString(9));
    }

    public async Task UpdateStatusAsync(long id, TicketStatus status, CancellationToken cancellationToken)
    {
        const string sql = """
                           update tickets
                           set status = @Status
                           where id = @Id
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("@Id", id));
        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "@Status",
            Value = status,
            DataTypeName = "ticket_status",
        });

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Ticket> GetAllTicketsByUserIdAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select id, user_id, event_id, price, row, number, status, payment_id, created_at, applied_promocode
                           from tickets
                           where user_id = @UserId
                           order by id asc
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Ticket(
                Id: reader.GetInt64(0),
                UserId: reader.GetInt64(1),
                EventId: reader.GetInt64(2),
                Price: reader.GetInt64(3),
                Row: reader.GetInt64(4),
                Number: reader.GetInt64(5),
                Status: reader.GetFieldValue<TicketStatus>(5),
                PaymentId: reader.GetInt64(6),
                CreatedAt: reader.GetFieldValue<DateTimeOffset>(7),
                AppliedPromocode: reader.IsDBNull(7) ? null : reader.GetString(8));
        }
    }
}