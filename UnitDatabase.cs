using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Incri1_Galang
{
    internal static class UnitDatabase
    {
        internal const string ApplicationTableName = "AppResponseUnits";
        internal const string ReportsTableName = "ResidentReports";

        public static event EventHandler? DataChanged;

        private static readonly string[] AccessProviderCandidates =
        {
            "Microsoft.ACE.OLEDB.16.0",
            "Microsoft.ACE.OLEDB.12.0"
        };

        private static readonly string[] AccessDatabaseCandidatePaths =
        {
            Path.Combine(AppContext.BaseDirectory, "DatabaseDispatch.accdb"),
            Path.Combine(AppContext.BaseDirectory, "data", "DatabaseDispatch.accdb"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DatabaseDispatch.accdb"))
        };

        private static readonly HashSet<string> RequiredColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UnitID",
            "UnitName",
            "UnitType",
            "Status",
            "Location",
            "DateAdded",
            "Resources",
            "IncidentLocation",
            "AlarmDescription"
        };

        public static void Initialize()
        {
            using OleDbConnection connection = OpenAccessConnection();
            EnsureTable(connection, ApplicationTableName);
            EnsureTable(connection, ReportsTableName);
        }

        public static string GetResolvedDatabasePath()
        {
            return ResolveAccessDatabasePath();
        }

        public static List<ResponseUnit> GetAllUnits()
        {
            return GetAllRows(ApplicationTableName);
        }

        public static List<ResponseUnit> GetAllReports()
        {
            return GetAllRows(ReportsTableName);
        }

        private static List<ResponseUnit> GetAllRows(string tableName)
        {
            List<ResponseUnit> units = new List<ResponseUnit>();

            using OleDbConnection connection = OpenAccessConnection();

            string sql = $@"
                SELECT [UnitID], [UnitName], [UnitType], [Status], [Location], [DateAdded], [Resources], [IncidentLocation], [AlarmDescription]
                FROM [{tableName}]
                ORDER BY [UnitID];";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            using OleDbDataReader reader = command.ExecuteReader();

            if (reader == null)
            {
                return units;
            }

            while (reader.Read())
            {
                DateTime parsedDate = DateTime.Now;
                if (!reader.IsDBNull(5))
                {
                    object dateValue = reader.GetValue(5);
                    if (dateValue is DateTime dt)
                    {
                        parsedDate = dt;
                    }
                    else if (DateTime.TryParse(dateValue?.ToString(), out DateTime parsed))
                    {
                        parsedDate = parsed;
                    }
                }

                units.Add(new ResponseUnit
                {
                    UnitID = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0)),
                    UnitName = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1)?.ToString() ?? string.Empty,
                    UnitType = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2)?.ToString() ?? string.Empty,
                    Status = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3)?.ToString() ?? string.Empty,
                    Location = reader.IsDBNull(4) ? string.Empty : reader.GetValue(4)?.ToString() ?? string.Empty,
                    DateAdded = parsedDate,
                    Resources = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6)?.ToString() ?? string.Empty,
                    IncidentLocation = reader.IsDBNull(7) ? string.Empty : reader.GetValue(7)?.ToString() ?? string.Empty,
                    AlarmDescription = reader.IsDBNull(8) ? string.Empty : reader.GetValue(8)?.ToString() ?? string.Empty
                });
            }

            return units;
        }

        public static int AddUnit(ResponseUnit unit)
        {
            try
            {
                return AddRow(ApplicationTableName, unit);
            }
            catch (OleDbException ex) when (IsDataTypeMismatch(ex))
            {
                using OleDbConnection fixConnection = OpenAccessConnection();
                RecreateTable(fixConnection, ApplicationTableName);
                return AddRow(ApplicationTableName, unit);
            }
        }

        public static int AddReport(ResponseUnit report)
        {
            try
            {
                return AddRow(ReportsTableName, report);
            }
            catch (OleDbException ex) when (IsDataTypeMismatch(ex))
            {
                using OleDbConnection fixConnection = OpenAccessConnection();
                RecreateTable(fixConnection, ReportsTableName);
                return AddRow(ReportsTableName, report);
            }
        }

        private static int AddRow(string tableName, ResponseUnit unit)
        {
            using OleDbConnection connection = OpenAccessConnection();

            string sql = $@"
                INSERT INTO [{tableName}] ([UnitName], [UnitType], [Status], [Location], [DateAdded], [Resources], [IncidentLocation], [AlarmDescription])
                VALUES (?, ?, ?, ?, ?, ?, ?, ?);";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = unit.UnitName;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = unit.UnitType;
            command.Parameters.Add("@p3", OleDbType.VarWChar, 255).Value = unit.Status;
            command.Parameters.Add("@p4", OleDbType.VarWChar, 255).Value = unit.Location;
            command.Parameters.Add("@p5", OleDbType.Date).Value = unit.DateAdded;
            command.Parameters.Add("@p6", OleDbType.LongVarWChar).Value = unit.Resources ?? string.Empty;
            command.Parameters.Add("@p7", OleDbType.LongVarWChar).Value = unit.IncidentLocation ?? string.Empty;
            command.Parameters.Add("@p8", OleDbType.LongVarWChar).Value = unit.AlarmDescription ?? string.Empty;
            command.ExecuteNonQuery();

            using OleDbCommand idCommand = new OleDbCommand("SELECT @@IDENTITY;", connection);
            object? insertedId = idCommand.ExecuteScalar();
            int newId = insertedId == null || insertedId == DBNull.Value ? 0 : Convert.ToInt32(insertedId);
            RaiseDataChanged();
            return newId;
        }

        public static void UpdateUnit(ResponseUnit unit)
        {
            UpdateRow(ApplicationTableName, unit);
        }

        public static void UpdateReport(ResponseUnit report)
        {
            UpdateRow(ReportsTableName, report);
        }

        private static void UpdateRow(string tableName, ResponseUnit unit)
        {
            using OleDbConnection connection = OpenAccessConnection();

            string sql = $@"
                UPDATE [{tableName}]
                SET [UnitName] = ?,
                    [UnitType] = ?,
                    [Status] = ?,
                    [Location] = ?,
                    [DateAdded] = ?,
                    [Resources] = ?,
                    [IncidentLocation] = ?,
                    [AlarmDescription] = ?
                WHERE [UnitID] = ?;";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = unit.UnitName;
            command.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = unit.UnitType;
            command.Parameters.Add("@p3", OleDbType.VarWChar, 255).Value = unit.Status;
            command.Parameters.Add("@p4", OleDbType.VarWChar, 255).Value = unit.Location;
            command.Parameters.Add("@p5", OleDbType.Date).Value = unit.DateAdded;
            command.Parameters.Add("@p6", OleDbType.LongVarWChar).Value = unit.Resources ?? string.Empty;
            command.Parameters.Add("@p7", OleDbType.LongVarWChar).Value = unit.IncidentLocation ?? string.Empty;
            command.Parameters.Add("@p8", OleDbType.LongVarWChar).Value = unit.AlarmDescription ?? string.Empty;
            command.Parameters.Add("@p9", OleDbType.Integer).Value = unit.UnitID;
            command.ExecuteNonQuery();
            RaiseDataChanged();
        }

        public static void DeleteUnit(int unitId)
        {
            DeleteRow(ApplicationTableName, unitId);
        }

        public static void DeleteReport(int reportId)
        {
            DeleteRow(ReportsTableName, reportId);
        }

        private static void DeleteRow(string tableName, int unitId)
        {
            using OleDbConnection connection = OpenAccessConnection();

            string sql = $"DELETE FROM [{tableName}] WHERE [UnitID] = ?;";
            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.Parameters.Add("@p1", OleDbType.Integer).Value = unitId;
            command.ExecuteNonQuery();
            RaiseDataChanged();
        }

        public static void ReplaceAllUnits(IEnumerable<ResponseUnit> units)
        {
            using OleDbConnection connection = OpenAccessConnection();
            using OleDbTransaction transaction = connection.BeginTransaction();

            using (OleDbCommand clearCommand = new OleDbCommand($"DELETE FROM [{ApplicationTableName}];", connection, transaction))
            {
                clearCommand.ExecuteNonQuery();
            }

            string insertSql = $@"
                INSERT INTO [{ApplicationTableName}] ([UnitName], [UnitType], [Status], [Location], [DateAdded], [Resources], [IncidentLocation], [AlarmDescription])
                VALUES (?, ?, ?, ?, ?, ?, ?, ?);";

            foreach (ResponseUnit unit in units)
            {
                using OleDbCommand insertCommand = new OleDbCommand(insertSql, connection, transaction);
                insertCommand.Parameters.Add("@p1", OleDbType.VarWChar, 255).Value = unit.UnitName;
                insertCommand.Parameters.Add("@p2", OleDbType.VarWChar, 255).Value = unit.UnitType;
                insertCommand.Parameters.Add("@p3", OleDbType.VarWChar, 255).Value = unit.Status;
                insertCommand.Parameters.Add("@p4", OleDbType.VarWChar, 255).Value = unit.Location;
                insertCommand.Parameters.Add("@p5", OleDbType.Date).Value = unit.DateAdded;
                insertCommand.Parameters.Add("@p6", OleDbType.LongVarWChar).Value = unit.Resources ?? string.Empty;
                insertCommand.Parameters.Add("@p7", OleDbType.LongVarWChar).Value = unit.IncidentLocation ?? string.Empty;
                insertCommand.Parameters.Add("@p8", OleDbType.LongVarWChar).Value = unit.AlarmDescription ?? string.Empty;
                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
            RaiseDataChanged();
        }

        private static void RaiseDataChanged()
        {
            DataChanged?.Invoke(null, EventArgs.Empty);
        }

        public static OleDbConnection CreateOpenConnection()
        {
            return OpenAccessConnection();
        }

        private static OleDbConnection OpenAccessConnection()
        {
            string databasePath = ResolveAccessDatabasePath();
            Exception? lastError = null;

            foreach (string provider in AccessProviderCandidates)
            {
                string connectionString = $"Provider={provider};Data Source={databasePath};Persist Security Info=False;";
                OleDbConnection connection = new OleDbConnection(connectionString);

                try
                {
                    connection.Open();
                    return connection;
                }
                catch (Exception ex)
                {
                    connection.Dispose();
                    lastError = ex;
                }
            }

            string errorMessage = "Microsoft Access provider was not found for this app process. Install Microsoft Access Database Engine matching app bitness (x86 app -> x86 engine).";
            if (lastError != null)
            {
                errorMessage += "\n\nLast provider error: " + lastError.Message;
            }

            throw new InvalidOperationException(errorMessage);
        }

        private static string ResolveAccessDatabasePath()
        {
            foreach (string candidate in AccessDatabaseCandidatePaths)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                "Access database file was not found. Expected one of: " + string.Join(" | ", AccessDatabaseCandidatePaths));
        }

        private static bool TableExists(OleDbConnection connection, string tableName)
        {
            DataTable? tables = connection.GetOleDbSchemaTable(
                OleDbSchemaGuid.Tables,
                new object?[] { null, null, tableName, "TABLE" });

            return tables != null && tables.Rows.Count > 0;
        }

        private static void EnsureTable(OleDbConnection connection, string tableName)
        {
            if (TableExists(connection, tableName))
            {
                if (!HasSchema(connection, tableName))
                {
                    RecreateTable(connection, tableName);
                }
                return;
            }

            CreateTable(connection, tableName);
        }

        private static bool HasSchema(OleDbConnection connection, string tableName)
        {
            DataTable? columns = connection.GetOleDbSchemaTable(
                OleDbSchemaGuid.Columns,
                new object?[] { null, null, tableName, null });

            if (columns == null)
            {
                return false;
            }

            HashSet<string> actualColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in columns.Rows)
            {
                string? name = row["COLUMN_NAME"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    actualColumns.Add(name);
                }
            }

            return RequiredColumns.IsSubsetOf(actualColumns);
        }

        private static void RecreateTable(OleDbConnection connection, string tableName)
        {
            using (OleDbCommand drop = new OleDbCommand($"DROP TABLE [{tableName}];", connection))
            {
                try
                {
                    drop.ExecuteNonQuery();
                }
                catch
                {
                    // Ignore when table cannot be dropped in the current state.
                }
            }

            CreateTable(connection, tableName);
        }

        private static void CreateTable(OleDbConnection connection, string tableName)
        {
            string sql = $@"
                CREATE TABLE [{tableName}] (
                    [UnitID] COUNTER PRIMARY KEY,
                    [UnitName] TEXT(255) NOT NULL,
                    [UnitType] TEXT(255) NOT NULL,
                    [Status] TEXT(255) NOT NULL,
                    [Location] TEXT(255) NOT NULL,
                    [DateAdded] DATETIME NOT NULL,
                    [Resources] LONGTEXT,
                    [IncidentLocation] LONGTEXT,
                    [AlarmDescription] LONGTEXT
                );";

            using OleDbCommand command = new OleDbCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static bool IsDataTypeMismatch(OleDbException ex)
        {
            return ex.Message.IndexOf("Data type mismatch", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
