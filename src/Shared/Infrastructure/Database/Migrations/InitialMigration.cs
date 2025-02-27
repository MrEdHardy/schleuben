using FluentMigrator;

namespace Shared.Infrastructure.Database.Migrations;

/// <summary>
/// Initial migration.
/// </summary>
[Migration(0, TransactionBehavior.None, "Initial Migration")]
public sealed class InitialMigration : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        this.Delete.Table("Person").IfExists();
        this.Delete.Table("Address").IfExists();
        this.Delete.Table("TelephoneConnection").IfExists();
    }

    /// <inheritdoc/>
    public override void Up()
    {
        this.IfDatabase(ProcessorId.SQLite).Delegate(() =>
        {
            this.Execute.Sql("PRAGMA foreign_keys = ON;");

            // Umstellung des Journal-Mode auf TRUNCATE, da Truncate schneller ist.
            // Hier könnte eventuell auch WAL verwendet werden, aber WAL ist nicht kompatibel mit Auto-Vacuum FULL.
            this.Execute.Sql("PRAGMA journal_mode = TRUNCATE;");

            // Umstellung auf Auto-Vacuum FULL und anschließendes VACUUM. Das ist laut SQLite-Doku notwendig.
            this.Execute.Sql("PRAGMA auto_vacuum  = FULL;");
            this.Execute.Sql("VACUUM");
        });

        this.Create.Table("Person")
            .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()
            .WithColumn("firstName")
                .AsString(100)
                .NotNullable()
            .WithColumn("lastName")
                .AsString(100)
                .NotNullable()
            .WithColumn("birthDate")
                .AsString(10)
                .Nullable();

        this.Create.Table("Address")
            .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()
            .WithColumn("street")
                .AsString(100)
                .NotNullable()
            .WithColumn("houseNumber")
                .AsString(10)
                .NotNullable()
            .WithColumn("city")
                .AsString(100)
                .NotNullable()
            .WithColumn("postCode")
                .AsString(10)
                .NotNullable()
            .WithColumn("additionalInfo")
                .AsString(100)
                .Nullable()
            .WithColumn("persId")
                .AsInt32()
                .NotNullable()
                .ForeignKey("Person", "id");

        this.Create.Table("TelephoneConnection")
            .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()
            .WithColumn("telephoneNumber")
                .AsString(20)
                .NotNullable()
            .WithColumn("persId")
                .AsInt32()
                .NotNullable()
                .ForeignKey("Person", "id");
    }
}
