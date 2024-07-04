Add-Migration -Context DatabaseContext -Project DataAccessLayer

































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
                                CREATE TRIGGER [dbo].Delete_Customer
                                ON [dbo].Customers
                                AFTER DELETE
                                AS
                                BEGIN
                                DELETE FROM [Identity].Users
                                WHERE [Id] IN (SELECT UId FROM DELETED);
                                END;');
                                """;

            var triggerInsert = """
                                IF NOT EXISTS (SELECT * FROM Sys.triggers WHERE NAME = '%_Customer')
                                EXEC ('
                                CREATE TRIGGER [Identity].Insert_Customer
                                ON [Identity].Users
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

