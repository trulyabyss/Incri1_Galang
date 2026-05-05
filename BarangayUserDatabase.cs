using System;
using System.Data;
using System.Data.OleDb;

namespace Incri1_Galang
{
    internal static class BarangayUserDatabase
    {
        private const string BarangayUserTableName = "BarangayUsers";

        public static void Initialize()
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            if (TableExists(connection, BarangayUserTableName))
            {
                return;
            }

            CreateBarangayUserTable(connection);
        }

        public static bool RegisterBarangayUser(string username, string password, string barangayName, out string error)
        {
            error = string.Empty;
            string normalized = NormalizeUsername(username);

            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(barangayName))
            {
                error = "Barangay name is required.";
                return false;
            }

            if (UsernameExists(normalized))
            {
                error = "Username already exists. Please choose another one.";
                return false;
            }

            AddUser(normalized, password, barangayName.Trim());
            return true;
        }

        public static bool TryGetBarangayForUser(string username, string password, out string barangayName, out string error)
        {
            barangayName = string.Empty;
            error = string.Empty;

            string normalized = NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"SELECT [Barangay] FROM [{BarangayUserTableName}] WHERE [Username] = ? AND [Password] = ?;";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = normalized;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = password;

            object? value = command.ExecuteScalar();
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            barangayName = value.ToString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(barangayName);
        }

        private static void AddUser(string username, string password, string barangayName)
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"INSERT INTO [{BarangayUserTableName}] ([Username], [Password], [Barangay]) VALUES (?, ?, ?);";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = username;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = password;
            command.Parameters.Add("@p3", OleDbType.VarWChar, 255).Value = barangayName;
            command.ExecuteNonQuery();
        }

        private static bool UsernameExists(string username)
        {
            using OleDbConnection connection = UnitDatabase.CreateOpenConnection();
            string sql = $"SELECT COUNT(*) FROM [{BarangayUserTableName}] WHERE [Username] = ?;";

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

        private static void CreateBarangayUserTable(OleDbConnection connection)
        {
            string sql = $@"
                CREATE TABLE [{BarangayUserTableName}] (
                    [UserID] COUNTER PRIMARY KEY,
                    [Username] TEXT(255) NOT NULL,
                    [Password] TEXT(255) NOT NULL,
                    [Barangay] TEXT(255) NOT NULL
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
