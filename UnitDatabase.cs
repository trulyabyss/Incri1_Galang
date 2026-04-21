using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Incri1_Galang
{
    internal static class UnitDatabase
    {
        private static readonly string DbPath = Path.Combine(AppContext.BaseDirectory, "incri1_galang.db");
        private static string ConnectionString => $"Data Source={DbPath}";

        public static void Initialize()
        {
            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS ResponseUnits (
                    UnitID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitName TEXT NOT NULL,
                    UnitType TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    DateAdded TEXT NOT NULL,
                    Resources TEXT,
                    IncidentLocation TEXT,
                    AlarmDescription TEXT
                );";

            using SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        public static List<ResponseUnit> GetAllUnits()
        {
            List<ResponseUnit> units = new List<ResponseUnit>();

            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = @"
                SELECT UnitID, UnitName, UnitType, Status, Location, DateAdded, Resources, IncidentLocation, AlarmDescription
                FROM ResponseUnits
                ORDER BY UnitID;";

            using SqliteCommand command = new SqliteCommand(sql, connection);
            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DateTime.TryParse(reader.GetString(5), out DateTime parsedDate);

                units.Add(new ResponseUnit
                {
                    UnitID = reader.GetInt32(0),
                    UnitName = reader.GetString(1),
                    UnitType = reader.GetString(2),
                    Status = reader.GetString(3),
                    Location = reader.GetString(4),
                    DateAdded = parsedDate == default ? DateTime.Now : parsedDate,
                    Resources = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    IncidentLocation = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    AlarmDescription = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                });
            }

            return units;
        }

        public static int AddUnit(ResponseUnit unit)
        {
            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = @"
                INSERT INTO ResponseUnits (UnitName, UnitType, Status, Location, DateAdded, Resources, IncidentLocation, AlarmDescription)
                VALUES ($name, $type, $status, $location, $dateAdded, $resources, $incidentLocation, $alarmDescription);
                SELECT last_insert_rowid();";

            using SqliteCommand command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$name", unit.UnitName);
            command.Parameters.AddWithValue("$type", unit.UnitType);
            command.Parameters.AddWithValue("$status", unit.Status);
            command.Parameters.AddWithValue("$location", unit.Location);
            command.Parameters.AddWithValue("$dateAdded", unit.DateAdded.ToString("o"));
            command.Parameters.AddWithValue("$resources", unit.Resources);
            command.Parameters.AddWithValue("$incidentLocation", unit.IncidentLocation);
            command.Parameters.AddWithValue("$alarmDescription", unit.AlarmDescription);

            long insertedId = (long)command.ExecuteScalar()!;
            return (int)insertedId;
        }

        public static void UpdateUnit(ResponseUnit unit)
        {
            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = @"
                UPDATE ResponseUnits
                SET UnitName = $name,
                    UnitType = $type,
                    Status = $status,
                    Location = $location,
                    DateAdded = $dateAdded,
                    Resources = $resources,
                    IncidentLocation = $incidentLocation,
                    AlarmDescription = $alarmDescription
                WHERE UnitID = $unitId;";

            using SqliteCommand command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$name", unit.UnitName);
            command.Parameters.AddWithValue("$type", unit.UnitType);
            command.Parameters.AddWithValue("$status", unit.Status);
            command.Parameters.AddWithValue("$location", unit.Location);
            command.Parameters.AddWithValue("$dateAdded", unit.DateAdded.ToString("o"));
            command.Parameters.AddWithValue("$resources", unit.Resources);
            command.Parameters.AddWithValue("$incidentLocation", unit.IncidentLocation);
            command.Parameters.AddWithValue("$alarmDescription", unit.AlarmDescription);
            command.Parameters.AddWithValue("$unitId", unit.UnitID);
            command.ExecuteNonQuery();
        }

        public static void DeleteUnit(int unitId)
        {
            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = "DELETE FROM ResponseUnits WHERE UnitID = $unitId;";
            using SqliteCommand command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$unitId", unitId);
            command.ExecuteNonQuery();
        }

        public static void ReplaceAllUnits(IEnumerable<ResponseUnit> units)
        {
            using SqliteConnection connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            using (SqliteCommand clearCommand = new SqliteCommand("DELETE FROM ResponseUnits;", connection, transaction))
            {
                clearCommand.ExecuteNonQuery();
            }

            // Reset the AUTOINCREMENT counter so imported UnitIDs start at 1 in sequence.
            using (SqliteCommand resetSequence = new SqliteCommand("DELETE FROM sqlite_sequence WHERE name = 'ResponseUnits';", connection, transaction))
            {
                resetSequence.ExecuteNonQuery();
            }

            const string insertSql = @"
                INSERT INTO ResponseUnits (UnitID, UnitName, UnitType, Status, Location, DateAdded, Resources, IncidentLocation, AlarmDescription)
                VALUES ($unitId, $name, $type, $status, $location, $dateAdded, $resources, $incidentLocation, $alarmDescription);";

            int nextUnitId = 1;
            foreach (ResponseUnit unit in units)
            {
                using SqliteCommand insertCommand = new SqliteCommand(insertSql, connection, transaction);
                insertCommand.Parameters.AddWithValue("$unitId", nextUnitId);
                insertCommand.Parameters.AddWithValue("$name", unit.UnitName);
                insertCommand.Parameters.AddWithValue("$type", unit.UnitType);
                insertCommand.Parameters.AddWithValue("$status", unit.Status);
                insertCommand.Parameters.AddWithValue("$location", unit.Location);
                insertCommand.Parameters.AddWithValue("$dateAdded", unit.DateAdded.ToString("o"));
                insertCommand.Parameters.AddWithValue("$resources", unit.Resources);
                insertCommand.Parameters.AddWithValue("$incidentLocation", unit.IncidentLocation);
                insertCommand.Parameters.AddWithValue("$alarmDescription", unit.AlarmDescription);
                insertCommand.ExecuteNonQuery();
                nextUnitId++;
            }

            int maxInsertedId = nextUnitId - 1;
            using (SqliteCommand saveSequence = new SqliteCommand("INSERT INTO sqlite_sequence(name, seq) VALUES ('ResponseUnits', $seq);", connection, transaction))
            {
                saveSequence.Parameters.AddWithValue("$seq", maxInsertedId);
                saveSequence.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }
}
