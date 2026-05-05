using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form15 : Form
    {
        private readonly BindingList<ResponseUnit> _reports = new BindingList<ResponseUnit>();
        private readonly BindingSource _bindingSource = new BindingSource();
        private readonly List<ResponseUnit> _allReports = new List<ResponseUnit>();

        public Form15()
        {
            InitializeComponent();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;

            _bindingSource.DataSource = _reports;
            dataGridView1.DataSource = _bindingSource;

            button1.Click += HandleShowData;
            button2.Click += HandleReportStatus;
            button3.Click += HandleRemoveReport;
            button10.Click += HandleViewUsers;
            button11.Click += HandleOpenChat;
            button6.Click += HandleLogout;
            searchBox.TextChanged += HandleSearchChanged;
            clearButton.Click += HandleClearSearch;

            LoadReports();

            UnitDatabase.DataChanged += HandleDataChanged;
            FormClosed += HandleFormClosed;
        }

        private void HandleDataChanged(object? sender, EventArgs e)
        {
            LoadReports();
        }

        private void HandleFormClosed(object? sender, FormClosedEventArgs e)
        {
            UnitDatabase.DataChanged -= HandleDataChanged;
            FormClosed -= HandleFormClosed;
        }

        private void HandleShowData(object? sender, EventArgs e)
        {
            using ReportsAnalyticsForm analyticsForm = new ReportsAnalyticsForm();
            analyticsForm.ShowDialog(this);
        }

        private void HandleLogout(object? sender, EventArgs e)
        {
            Close();
        }

        private void HandleSearchChanged(object? sender, EventArgs e)
        {
            ApplySearchFilter();
        }

        private void HandleClearSearch(object? sender, EventArgs e)
        {
            searchBox.Clear();
        }

        private void HandleReportStatus(object? sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedReport();
            if (selected == null)
            {
                MessageBox.Show("Select a report row first.", "Report Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using Form16 updateForm = new Form16(selected);
            if (updateForm.ShowDialog(this) == DialogResult.OK)
            {
                UnitDatabase.UpdateReport(selected);
                LoadReports();
            }
        }

        private void HandleRemoveReport(object? sender, EventArgs e)
        {
            ResponseUnit? selected = GetSelectedReport();
            if (selected == null)
            {
                MessageBox.Show("Select a report row first.", "Remove Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool removeFromDatabase = radioRemoveDatabase.Checked;
            string scopeText = removeFromDatabase ? "database" : "view only";
            DialogResult confirm = MessageBox.Show(
                $"Remove report {selected.UnitID} ({selected.UnitName}) from {scopeText}?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            if (removeFromDatabase)
            {
                UnitDatabase.DeleteReport(selected.UnitID);
                LoadReports();
            }
            else
            {
                RemoveReportFromView(selected);
            }
        }

        private void RemoveReportFromView(ResponseUnit selected)
        {
            if (selected == null)
            {
                return;
            }

            _allReports.RemoveAll(unit => unit.UnitID == selected.UnitID);
            ApplySearchFilter();
        }

        private void HandleViewUsers(object? sender, EventArgs e)
        {
            try
            {
                UserDatabase.Initialize();
                List<string> users = UserDatabase.GetAllUsernames();

                if (users.Count == 0)
                {
                    MessageBox.Show("No registered users found.",
                        "Registered Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string list = string.Join(Environment.NewLine, users);
                MessageBox.Show(list, "Registered Users", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load users. " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleOpenChat(object? sender, EventArgs e)
        {
            using ChatForm chatForm = new ChatForm(ChatRole.Barangay);
            chatForm.ShowDialog(this);
        }

        private void LoadReports()
        {
            UnitDatabase.Initialize();
            List<ResponseUnit> reports = UnitDatabase.GetAllReports();

            _allReports.Clear();
            _allReports.AddRange(reports);
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            string term = searchBox.Text.Trim();
            IEnumerable<ResponseUnit> filtered = string.IsNullOrWhiteSpace(term)
                ? _allReports
                : _allReports.Where(unit => MatchesSearch(unit, term));

            _reports.Clear();
            foreach (ResponseUnit unit in filtered)
            {
                _reports.Add(unit);
            }

            _bindingSource.ResetBindings(false);
            ApplyUnitColumnHeaders(dataGridView1);
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.AutoResizeColumns();
            }
        }

        private ResponseUnit? GetSelectedReport()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is ResponseUnit current)
            {
                return current;
            }

            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].DataBoundItem is ResponseUnit selected)
            {
                return selected;
            }

            if (_bindingSource.Current is ResponseUnit boundCurrent)
            {
                return boundCurrent;
            }

            if (_reports.Count > 0)
            {
                return _reports[0];
            }

            return null;
        }

        private static bool MatchesSearch(ResponseUnit unit, string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return true;
            }

            return unit.UnitName.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.UnitType.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.Status.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.Location.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.IncidentLocation.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.Resources.Contains(term, StringComparison.OrdinalIgnoreCase)
                || unit.AlarmDescription.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyUnitColumnHeaders(DataGridView grid)
        {
            DataGridViewColumn? unitNameColumn = grid.Columns["UnitName"];
            if (unitNameColumn != null)
            {
                unitNameColumn.Visible = false;
            }

            DataGridViewColumn? unitTypeColumn = grid.Columns["UnitType"];
            if (unitTypeColumn != null)
            {
                unitTypeColumn.Visible = false;
            }

            DataGridViewColumn? unitIdColumn = grid.Columns["UnitID"];
            if (unitIdColumn != null)
            {
                unitIdColumn.HeaderText = "UNITID";
                unitIdColumn.DisplayIndex = 0;
            }

            DataGridViewColumn? statusColumn = grid.Columns["Status"];
            if (statusColumn != null)
            {
                statusColumn.HeaderText = "STATUS";
                statusColumn.DisplayIndex = 1;
            }

            DataGridViewColumn? incidentColumn = grid.Columns["IncidentLocation"];
            if (incidentColumn != null)
            {
                incidentColumn.HeaderText = "SITIO";
                incidentColumn.DisplayIndex = 2;
            }

            DataGridViewColumn? locationColumn = grid.Columns["Location"];
            if (locationColumn != null)
            {
                locationColumn.HeaderText = "BARANGAY";
                locationColumn.DisplayIndex = 3;
            }

            DataGridViewColumn? dateAddedColumn = grid.Columns["DateAdded"];
            if (dateAddedColumn != null)
            {
                dateAddedColumn.HeaderText = "DATEADDED";
                dateAddedColumn.DisplayIndex = 4;
            }

            DataGridViewColumn? resourcesColumn = grid.Columns["Resources"];
            if (resourcesColumn != null)
            {
                resourcesColumn.HeaderText = "RESOURCES";
                resourcesColumn.DisplayIndex = 5;
            }

            DataGridViewColumn? alarmColumn = grid.Columns["AlarmDescription"];
            if (alarmColumn != null)
            {
                alarmColumn.HeaderText = "ALARMDESCRIPTION";
                alarmColumn.DisplayIndex = 6;
            }
        }
    }
}
