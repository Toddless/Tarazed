#nullable disable

namespace DataAccessLayer.Migrations.ApplicationDb
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class LeeresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Identity",
                table: "AspNetUsers");
        }

        private string GenerateSQL(string action, string triggerAction)
        {
            if (string.IsNullOrWhiteSpace(action) || triggerAction == null)
            {
                throw new ArgumentException("Is null or empty");
            }

            action = action.ToUpper();
            triggerAction = triggerAction.ToUpper();

            var triggerDrop = """
                              DROP TRIGGER IF EXISTS [Identity].Insert_Customer;
                              DROP TRIGGER IF EXISTS [Identity].Delete_Customer;
                              """;

            var triggerDelete = $"""
                                 IF NOT EXISTS (SELECT * FROM Sys.triggers WHERE NAME = '%_Customer')
                                 EXEC ('
                                 CREATE TRIGGER [Identity].Delete_Customer
                                 ON [Identity].AspNetUsers
                                 AFTER DELETE
                                 AS
                                 BEGIN
                                 DELETE FROM Customers
                                 WHERE [UId] IN (SELECT Id FROM DELETED);
                                 END;');
                                """;

            var triggerInsert = """
                                IF NOT EXISTS (SELECT * FROM Sys.triggers WHERE NAME = '%_Customer')
                                EXEC ('
                                CREATE TRIGGER [Identity].Insert_Customer
                                ON [Identity].AspNetUsers
                                AFTER INSERT
                                AS
                                BEGIN 
                                INSERT INTO Customers (Name, Email, PasswortHash, UId)
                                SELECT UserName, Email, PasswordHash, Id
                                FROM INSERTED;
                                END;');
                                """;

            return action switch
            {
                "CREATE" => triggerAction switch
                {
                    "INSERT" => triggerInsert,
                    "DELETE" => triggerDelete,
                    _ => throw new ArgumentException("Wrong action.")
                },
                "DROP" => triggerDrop,
                _ => throw new ArgumentException("Wrong action.")
            };
        }
    }
}
