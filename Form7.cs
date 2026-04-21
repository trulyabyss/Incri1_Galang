using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form7 : Form
    {
        private const string AccessDatabasePath = @"E:\2026GALANG SECONDSEM FINALPROJECT\DatabaseDispatch.accdb";

        // Static list ensures data persists across all forms
        public static BindingList<ResponseUnit> GlobalUnitList = new BindingList<ResponseUnit>();
        private readonly BindingSource _unitBindingSource = new BindingSource();

        public Form7()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            _unitBindingSource.DataSource = GlobalUnitList;
            dataGridView1.DataSource = _unitBindingSource;
            UnitDatabase.Initialize();
            LoadUnitsFromDatabase();
        }

        public void RefreshDataGrid()
        {
            _unitBindingSource.ResetBindings(false);

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.AutoResizeColumns();
            }
        }

        private void LoadUnitsFromDatabase()
        {
            List<ResponseUnit> units = UnitDatabase.GetAllUnits();

            GlobalUnitList.Clear();
            foreach (ResponseUnit unit in units)
            {
                GlobalUnitList.Add(unit);
            }

            RefreshDataGrid();
        }

        private ResponseUnit? GetSelectedUnit()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is ResponseUnit current)
            {
                return current;
            }

            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].DataBoundItem is ResponseUnit selected)
            {
                return selected;
            }

            if (_unitBindingSource.Current is ResponseUnit boundCurrent)
            {
                return boundCurrent;
            }

            if (GlobalUnitList.Count > 0)
            {
                return GlobalUnitList[0];
            }

            return null;
        }

        private static Dictionary<string, int> BuildColumnIndexMap(IDataRecord record)
        {
            Dictionary<string, int> indexMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < record.FieldCount; i++)
            {
                string name = record.GetName(i);
                if (!string.IsNullOrWhiteSpace(name) && !indexMap.ContainsKey(name))
                {
                    indexMap[name] = i;
                }
            }

            return indexMap;
        }

        private static string ReadStringValue(IDataRecord record, IReadOnlyDictionary<string, int> indexMap, params string[] candidateNames)
        {
            foreach (string name in candidateNames)
            {
                if (indexMap.TryGetValue(name, out int index) && !record.IsDBNull(index))
                {
                    object value = record.GetValue(index);
                    return value?.ToString()?.Trim() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static DateTime ReadDateValue(IDataRecord record, IReadOnlyDictionary<string, int> indexMap, params string[] candidateNames)
        {
            foreach (string name in candidateNames)
            {
                if (indexMap.TryGetValue(name, out int index) && !record.IsDBNull(index))
                {
                    object value = record.GetValue(index);
                    if (value is DateTime dt)
                    {
                        return dt;
                    }

                    if (DateTime.TryParse(value?.ToString(), out DateTime parsed))
                    {
                        return parsed;
                    }
                }
            }

            return DateTime.Now;
        }

        private static HashSet<string> GetAccessTableColumns(OleDbConnection connection, string tableName)
        {
            DataTable? columns = connection.GetOleDbSchemaTable(
                OleDbSchemaGuid.Columns,
                new object?[] { null, null, tableName, null });

            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (columns == null)
            {
                return names;
            }

            foreach (DataRow row in columns.Rows)
            {
                string? col = row["COLUMN_NAME"]?.ToString();
                if (!string.IsNullOrWhiteSpace(col))
                {
                    names.Add(col);
                }
            }

            return names;
        }

        private static int GetAccessTableRowCount(OleDbConnection connection, string tableName)
        {
            try
            {
                using OleDbCommand command = new OleDbCommand($"SELECT COUNT(*) FROM [{tableName}]", connection);
                object? value = command.ExecuteScalar();
                return Convert.ToInt32(value ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        private static int ComputeAccessTableScore(HashSet<string> columns)
        {
            int score = 0;

            if (columns.Contains("UnitName") || columns.Contains("Unit Name") || columns.Contains("Name") || columns.Contains("Unit")) score += 6;
            if (columns.Contains("UnitType") || columns.Contains("Type") || columns.Contains("UnitBelongTo") || columns.Contains("Category")) score += 4;
            if (columns.Contains("Status") || columns.Contains("UnitStatus") || columns.Contains("Availability")) score += 3;
            if (columns.Contains("Location") || columns.Contains("Station") || columns.Contains("StationLocation") || columns.Contains("Address")) score += 3;
            if (columns.Contains("DateAdded") || columns.Contains("Date") || columns.Contains("CreatedAt")) score += 2;
            if (columns.Contains("Resources") || columns.Contains("Resource")) score += 1;

            return score;
        }

        private static string? GetPreferredAccessTableName(OleDbConnection connection)
        {
            DataTable? tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object?[] { null, null, null, "TABLE" });
            if (tables == null || tables.Rows.Count == 0)
            {
                return null;
            }

            List<string> names = new List<string>();
            foreach (DataRow row in tables.Rows)
            {
                string? tableName = row["TABLE_NAME"]?.ToString();
                if (!string.IsNullOrWhiteSpace(tableName) && !tableName.StartsWith("MSys", StringComparison.OrdinalIgnoreCase))
                {
                    names.Add(tableName);
                }
            }

            if (names.Count == 0)
            {
                return null;
            }

            string[] preferred = { "ResponseUnits", "Units", "Unit", "DispatchUnits", "tblUnits", "tblResponseUnits" };
            foreach (string preferredName in preferred)
            {
                string? found = names.FirstOrDefault(n => string.Equals(n, preferredName, StringComparison.OrdinalIgnoreCase));
                if (found != null)
                {
                    return found;
                }
            }

            string bestName = names[0];
            int bestScore = -1;
            int bestRows = -1;

            foreach (string tableName in names)
            {
                HashSet<string> columns = GetAccessTableColumns(connection, tableName);
                int score = ComputeAccessTableScore(columns);
                int rows = GetAccessTableRowCount(connection, tableName);

                if (score > bestScore || (score == bestScore && rows > bestRows))
                {
                    bestScore = score;
                    bestRows = rows;
                    bestName = tableName;
                }
            }

            return bestName;
        }

        private void LoadUnitsFromAccessDatabase()
        {
            if (!File.Exists(AccessDatabasePath))
            {
                throw new FileNotFoundException("Access database file was not found.", AccessDatabasePath);
            }

            string[] providerCandidates =
            {
                "Microsoft.ACE.OLEDB.16.0",
                "Microsoft.ACE.OLEDB.12.0"
            };

            List<ResponseUnit> importedUnits = new List<ResponseUnit>();
            Exception? lastOpenError = null;

            using OleDbConnection connection = OpenAccessConnection(AccessDatabasePath, providerCandidates, out lastOpenError);

            string? tableName = GetPreferredAccessTableName(connection);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new InvalidOperationException("No usable table was found in the Access database.");
            }

            using OleDbCommand command = new OleDbCommand($"SELECT * FROM [{tableName}]", connection);
            using OleDbDataReader reader = command.ExecuteReader();

            if (reader == null)
            {
                throw new InvalidOperationException("Unable to read rows from the selected Access table.");
            }

            while (reader.Read())
            {
                Dictionary<string, int> indexMap = BuildColumnIndexMap(reader);

                ResponseUnit unit = new ResponseUnit
                {
                    UnitName = ReadStringValue(reader, indexMap, "UnitName", "Unit Name", "Name", "Unit", "ResponderName", "VehicleName"),
                    UnitType = ReadStringValue(reader, indexMap, "UnitType", "Type", "UnitBelongTo", "Unit Belong To", "Category"),
                    Status = ReadStringValue(reader, indexMap, "Status", "UnitStatus", "Availability"),
                    Location = ReadStringValue(reader, indexMap, "Location", "Station", "StationLocation", "StationedAt", "Address", "BaseLocation"),
                    DateAdded = ReadDateValue(reader, indexMap, "DateAdded", "Date", "Date Added", "CreatedAt"),
                    Resources = ReadStringValue(reader, indexMap, "Resources", "Resource", "Team", "Asset", "Assets"),
                    IncidentLocation = ReadStringValue(reader, indexMap, "IncidentLocation", "Incident", "Incident Location"),
                    AlarmDescription = ReadStringValue(reader, indexMap, "AlarmDescription", "Alarm", "Alarm Description")
                };

                bool isCompletelyEmpty =
                    string.IsNullOrWhiteSpace(unit.UnitName) &&
                    string.IsNullOrWhiteSpace(unit.UnitType) &&
                    string.IsNullOrWhiteSpace(unit.Status) &&
                    string.IsNullOrWhiteSpace(unit.Location) &&
                    string.IsNullOrWhiteSpace(unit.Resources) &&
                    string.IsNullOrWhiteSpace(unit.IncidentLocation) &&
                    string.IsNullOrWhiteSpace(unit.AlarmDescription);

                if (isCompletelyEmpty)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(unit.UnitName))
                {
                    unit.UnitName = $"IMPORTED UNIT {importedUnits.Count + 1}";
                }

                if (string.IsNullOrWhiteSpace(unit.UnitType))
                {
                    unit.UnitType = "BFP";
                }

                if (string.IsNullOrWhiteSpace(unit.Status))
                {
                    unit.Status = "Available";
                }

                if (string.IsNullOrWhiteSpace(unit.Location))
                {
                    unit.Location = "UNKNOWN";
                }

                importedUnits.Add(unit);
            }

            if (importedUnits.Count == 0)
            {
                throw new InvalidOperationException($"No importable records were found in table '{tableName}'.");
            }

            UnitDatabase.ReplaceAllUnits(importedUnits);
        }

        private static OleDbConnection OpenAccessConnection(string databasePath, IEnumerable<string> providerCandidates, out Exception? lastError)
        {
            lastError = null;

            foreach (string provider in providerCandidates)
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

            string msg = "Microsoft Access provider was not found for this app process. Install Microsoft Access Database Engine (2010/2016) matching app bitness (x64 app -> x64 engine, x86 app -> x86 engine).";
            if (lastError != null)
            {
                msg += "\n\nLast provider error: " + lastError.Message;
            }

            throw new InvalidOperationException(msg);
        }

        // ADD UNIT
        // Inside Form7.cs
        private void button1_Click(object sender, EventArgs e)
        {
            Form8 f8 = new Form8();

            // ShowDialog pauses Form7. Execution continues only after Form8 closes.
            if (f8.ShowDialog() == DialogResult.OK)
            {
                LoadUnitsFromDatabase();
            }
        }

        // UNIT STATUS
        private void button2_Click(object sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedUnit();
            if (selected != null)
            {
                Form9 f9 = new Form9(selected);
                if (f9.ShowDialog() == DialogResult.OK)
                {
                    UnitDatabase.UpdateUnit(selected);
                    LoadUnitsFromDatabase();
                }
                return;
            }

            MessageBox.Show("No units found. Add a unit first.", "No Units", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // DISPATCH UNIT
        private void button3_Click(object sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedUnit();
            if (selected != null)
            {
                if (!string.Equals(selected.Status, "Available", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Only units with status 'Available' can be dispatched.", "Dispatch Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Form10 f10 = new Form10(selected);
                if (f10.ShowDialog() == DialogResult.OK)
                {
                    UnitDatabase.UpdateUnit(selected);
                    LoadUnitsFromDatabase();
                }

                return;
            }

            MessageBox.Show("No units found. Add a unit first.", "No Units", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // VIEW ALL UNITS
        private void button4_Click(object sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedUnit();
            if (selected == null)
            {
                MessageBox.Show("No units found. Add a unit first.", "No Units", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.Equals(selected.Status, "Unavailable", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Only unavailable units can return to base.", "Return To Base", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            selected.Status = "Available";
            selected.IncidentLocation = string.Empty;
            selected.AlarmDescription = string.Empty;

            UnitDatabase.UpdateUnit(selected);
            LoadUnitsFromDatabase();
            MessageBox.Show($"Unit {selected.UnitID} ({selected.UnitName}) is now back to base.", "Return To Base", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // REMOVE UNITS
        private void button5_Click(object sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedUnit();
            if (selected == null)
            {
                MessageBox.Show("No units found to remove.", "Remove Units", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Remove Unit {selected.UnitID} ({selected.UnitName})?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            UnitDatabase.DeleteUnit(selected.UnitID);
            LoadUnitsFromDatabase();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close(); // Logout
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedUnit();
            Form11 mapForm = new Form11(selected);
            mapForm.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LoadUnitsFromDatabase();
            Form showDataForm = new Form
            {
                Text = "SHOW DATA",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(980, 520),
                MinimumSize = new Size(900, 460)
            };

            Label summaryLabel = new Label
            {
                AutoSize = true,
                Location = new Point(16, 14),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            DataGridView dataGrid = new DataGridView
            {
                Location = new Point(16, 40),
                Size = new Size(932, 395),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
                RowHeadersVisible = false,
                DataSource = null
            };

            void RefreshShowData()
            {
                List<ResponseUnit> units = UnitDatabase.GetAllUnits();
                dataGrid.DataSource = null;
                dataGrid.DataSource = units;

                int available = units.Count(u => string.Equals(u.Status, "Available", StringComparison.OrdinalIgnoreCase));
                int unavailable = units.Count(u => string.Equals(u.Status, "Unavailable", StringComparison.OrdinalIgnoreCase));
                summaryLabel.Text = $"Total Units: {units.Count}   |   Available: {available}   |   Unavailable: {unavailable}";
            }

            ResponseUnit? GetSelectedShowDataUnit()
            {
                if (dataGrid.CurrentRow?.DataBoundItem is ResponseUnit current)
                {
                    return current;
                }

                if (dataGrid.SelectedRows.Count > 0 && dataGrid.SelectedRows[0].DataBoundItem is ResponseUnit selected)
                {
                    return selected;
                }

                return null;
            }

            Button loadDataButton = new Button
            {
                Text = "LOAD DATA",
                Size = new Size(90, 28),
                Location = new Point(460, 445),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            loadDataButton.Click += (_, _) =>
            {
                try
                {
                    LoadUnitsFromAccessDatabase();
                    LoadUnitsFromDatabase();
                    RefreshShowData();
                    MessageBox.Show("Data loaded from Microsoft Access successfully.", "Load Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load data from Access.\n{ex.Message}", "Load Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button createButton = new Button
            {
                Text = "CREATE",
                Size = new Size(90, 28),
                Location = new Point(560, 445),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            createButton.Click += (_, _) =>
            {
                using Form8 addForm = new Form8();
                if (addForm.ShowDialog(showDataForm) == DialogResult.OK)
                {
                    LoadUnitsFromDatabase();
                    RefreshShowData();
                }
            };

            Button updateButton = new Button
            {
                Text = "UPDATE",
                Size = new Size(90, 28),
                Location = new Point(660, 445),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            updateButton.Click += (_, _) =>
            {
                ResponseUnit? selected = GetSelectedShowDataUnit();
                if (selected == null)
                {
                    MessageBox.Show("Select a unit row first.", "Update Unit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using Form9 editForm = new Form9(selected);
                if (editForm.ShowDialog(showDataForm) == DialogResult.OK)
                {
                    UnitDatabase.UpdateUnit(selected);
                    LoadUnitsFromDatabase();
                    RefreshShowData();
                }
            };

            Button deleteButton = new Button
            {
                Text = "DELETE",
                Size = new Size(90, 28),
                Location = new Point(760, 445),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            deleteButton.Click += (_, _) =>
            {
                ResponseUnit? selected = GetSelectedShowDataUnit();
                if (selected == null)
                {
                    MessageBox.Show("Select a unit row first.", "Delete Unit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    $"Delete Unit {selected.UnitID} ({selected.UnitName})?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                UnitDatabase.DeleteUnit(selected.UnitID);
                LoadUnitsFromDatabase();
                RefreshShowData();
            };

            Button closeButton = new Button
            {
                Text = "CLOSE",
                Size = new Size(90, 28),
                Location = new Point(858, 445),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            closeButton.Click += (_, _) => showDataForm.Close();

            RefreshShowData();
            showDataForm.Controls.Add(summaryLabel);
            showDataForm.Controls.Add(dataGrid);
            showDataForm.Controls.Add(loadDataButton);
            showDataForm.Controls.Add(createButton);
            showDataForm.Controls.Add(updateButton);
            showDataForm.Controls.Add(deleteButton);
            showDataForm.Controls.Add(closeButton);
            showDataForm.ShowDialog(this);
        }
    }
}