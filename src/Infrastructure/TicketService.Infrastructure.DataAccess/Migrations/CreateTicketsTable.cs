using FluentMigrator;

namespace TicketService.Infrastructure.DataAccess.Migrations;

[Migration(1, "create tickets table")]
public class CreateTicketsTable : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE TYPE ticket_status AS ENUM('reserved','paid','cancelled','refunded');");

        Create.Table("tickets")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("event_id").AsInt64().NotNullable()
            .WithColumn("price").AsInt64().NotNullable()
            .WithColumn("row").AsInt64().NotNullable()
            .WithColumn("number").AsInt64().NotNullable()
            .WithColumn("status").AsCustom("ticket_status").NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("payment_id").AsInt64().NotNullable()
            .WithColumn("applied_promocode").AsString().Nullable();
    }

    public override void Down()
    {
        Delete.Table("tickets");
        Execute.Sql("DROP TYPE IF EXISTS payment_status;");
    }
}