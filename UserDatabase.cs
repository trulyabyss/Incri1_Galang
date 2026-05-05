using System;
using System.Data;
using System.Data.OleDb;

namespace Incri1_Galang
{
    internal static class UserDatabase
    {
        private const string UserTableName = "AppUsers";

        public static void Initialize()
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            if (TableExists(connection, UserTableName))
            {
                return;
            }

            CreateUserTable(connection);
        }

        public static bool RegisterUser(string username, string password, out string error)
        {
            error = string.Empty;
            string normalized = NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            if (UsernameExists(normalized))
            {
                error = "Username already exists. Please choose another one.";
                return false;
            }

            AddUser(normalized, password);
            return true;
        }

        public static bool ValidateUser(string username, string password)
        {
            string normalized = NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"SELECT COUNT(*) FROM [{UserTableName}] WHERE [Username] = ? AND [Password] = ?;";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = normalized;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = password;

            object? value = command.ExecuteScalar();
            int count = Convert.ToInt32(value ?? 0);
            return count > 0;
        }

        public static List<string> GetAllUsernames()
        {
            List<string> users = new List<string>();

            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"SELECT [Username] FROM [{UserTableName}] ORDER BY [Username];";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            using OleDbDataReader reader = command.ExecuteReader();

            if (reader == null)
            {
                return users;
            }

            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    string name = reader.GetValue(0)?.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        users.Add(name);
                    }
                }
            }

            return users;
        }

        private static void AddUser(string username, string password)
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"INSERT INTO [{UserTableName}] ([Username], [Password]) VALUES (?, ?);";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = username;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = password;
            command.ExecuteNonQuery();
        }

        private static bool UsernameExists(string username)
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"SELECT COUNT(*) FROM [{UserTableName}] WHERE [Username] = ?;";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = username;
            object? value = command.ExecuteScalar();
            int count = Convert.ToInt32(value ?? 0);
            return count > 0;
        }

        private static bool TableExists(OleDbConnection connection, string tableName)
        {
            DataTable? tables = connection.GetOleDbSchemaTable(
                OleDbSchemaGuid.Tables,
                new object?[] { null, null, tableName, "TABLE" });

            return tables != null && tables.Rows.Count > 0;
        }

        private static void CreateUserTable(OleDbConnection connection)
        {
            string sql = $@"
                CREATE TABLE [{UserTableName}] (
                    [UserID] COUNTER PRIMARY KEY,
                    [Username] TEXT(255) NOT NULL,
                    [Password] TEXT(255) NOT NULL
                );";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static string NormalizeUsername(string username)
        {
            return (username ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}
