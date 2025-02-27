using FluentMigrator;

namespace Shared.Infrastructure.Database.Migrations;

/// <summary>
/// Exemplary data.
/// </summary>
[Migration(1, "Exemplary data")]
public sealed class ExampleDataMigration : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        this.Delete.FromTable("Address").AllRows();
        this.Delete.FromTable("TelephoneConnection").AllRows();
        this.Delete.FromTable("Person").AllRows();
    }

    /// <inheritdoc/>
    public override void Up()
    {
        this.IfDatabase(ProcessorId.SQLite).Delegate(() =>
        {
            this.Execute.Sql("PRAGMA foreign_keys = ON;");
        });

        this.Insert.IntoTable("Person")
            .Row(new { id = 1, firstName = "Max", lastName = "Mustermann", birthDate = "1990-01-01" })
            .Row(new { id = 2, firstName = "Peter", lastName = "Lustig", birthDate = "1945-01-01" });

        this.Insert.IntoTable("Address")
            .Row(new { id = 1, street = "Musterstraße", houseNumber = "1", city = "Musterstadt", postCode = "12345", persId = 1 })
            .Row(new { id = 2, street = "Lustigstraße", houseNumber = "2", city = "Lustigstadt", postCode = "54321", persId = 2 });

        this.Insert.IntoTable("TelephoneConnection")
            .Row(new { id = 1, telephoneNumber = "0123456789", persId = 1 })
            .Row(new { id = 2, telephoneNumber = "9876543210", persId = 2 });
    }
}
