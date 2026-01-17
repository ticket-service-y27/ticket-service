using FluentMigrator;

namespace TicketService.Infrastructure.DataAccess.Migrations;

[Migration(2, "create promocodes table")]
public class CreatePromocodesTable : Migration
{
    public override void Up()
    {
        Create.Table("promocodes")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().NotNullable().Unique()
            .WithColumn("discount_percentage").AsInt64().NotNullable()
            .WithColumn("count").AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("promocodes");
    }
}