using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form7 : Form
    {
        // Central in-memory storage for your units
        public static List<ResponseUnit> GlobalUnitList = new List<ResponseUnit>();

        public Form7()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            // Initialize the DataGridView on startup
            RefreshDataGrid();
        }

        // Helper function to update the table display
        public void RefreshDataGrid()
        {
            var orderedUnits = GlobalUnitList
                .OrderBy(u => u.UnitID)
                .ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = orderedUnits;

            // Hide the Location column
            var locationColumn = dataGridView1.Columns["Location"];
            if (locationColumn != null)
            {
                locationColumn.Visible = false;
            }

            // Fix the empty/header column (RowHeaders)
            dataGridView1.RowHeadersVisible = false;

            // Ensure UnitID column is visible and has a proper header if needed
            if (dataGridView1.Columns["UnitID"] != null)
            {
                dataGridView1.Columns["UnitID"].HeaderText = "Unit ID";
                dataGridView1.Columns["UnitID"].DisplayIndex = 0;
            }
        }

        private ResponseUnit? GetSelectedUnit()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a unit first.", "No Unit Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (dataGridView1.CurrentRow.DataBoundItem is not ResponseUnit unit)
            {
                MessageBox.Show("Unable to read the selected unit.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return unit;
        }

        private void RenumberUnitIds()
        {
            for (int i = 0; i < GlobalUnitList.Count; i++)
            {
                GlobalUnitList[i].UnitID = i + 1;
            }
        }

        private string? PromptForUnitStatus(string currentStatus)
        {
            using Form statusDialog = new Form();
            statusDialog.Text = "UNIT STATUS";
            statusDialog.StartPosition = FormStartPosition.CenterParent;
            statusDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            statusDialog.MinimizeBox = false;
            statusDialog.MaximizeBox = false;
            statusDialog.ClientSize = new System.Drawing.Size(300, 140);
            statusDialog.ShowInTaskbar = false;

            Label promptLabel = new Label
            {
                Text = "Select new unit status:",
                AutoSize = true,
                Location = new System.Drawing.Point(18, 18)
            };

            ComboBox statusComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(18, 45),
                Width = 260
            };
            statusComboBox.Items.AddRange(new object[] { "Available", "Maintenance", "Unavailable" });

            if (statusComboBox.Items.Contains(currentStatus))
            {
                statusComboBox.SelectedItem = currentStatus;
            }
            else
            {
                statusComboBox.SelectedItem = "Available";
            }

            Button okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(122, 92),
                Width = 75
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(203, 92),
                Width = 75
            };

            statusDialog.Controls.Add(promptLabel);
            statusDialog.Controls.Add(statusComboBox);
            statusDialog.Controls.Add(okButton);
            statusDialog.Controls.Add(cancelButton);

            statusDialog.AcceptButton = okButton;
            statusDialog.CancelButton = cancelButton;

            return statusDialog.ShowDialog(this) == DialogResult.OK
                ? statusComboBox.SelectedItem?.ToString()
                : null;
        }

        // ADD UNIT (Proceeds to Form 8)
        private void button1_Click(object? sender, EventArgs e)
        {
            Form8 addForm = new Form8();
            addForm.ShowDialog(); // Form 7 waits here until Form 8 is closed
            RefreshDataGrid();    // Refresh grid once you come back from Form 8
        }

        // UNIT STATUS (Toggles status of selected unit)
        private void button2_Click(object? sender, EventArgs e)
        {
            if (GlobalUnitList.Count == 0)
            {
                MessageBox.Show("There are no units to update yet.", "Unit Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var unit = GetSelectedUnit();
            if (unit == null)
            {
                return;
            }

            var selectedStatus = PromptForUnitStatus(unit.Status);
            if (string.IsNullOrWhiteSpace(selectedStatus))
            {
                return;
            }

            unit.Status = selectedStatus;

            RefreshDataGrid();
            MessageBox.Show($"{unit.UnitName} status is now {unit.Status}.", "Unit Status Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // DISPATCH UNIT
        private void button3_Click(object? sender, EventArgs e)
        {
            if (GlobalUnitList.Count == 0)
            {
                MessageBox.Show("There are no units to dispatch yet.", "Dispatch Unit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var unit = GetSelectedUnit();
            if (unit == null)
            {
                return;
            }

            if (unit.Status != "Available")
            {
                MessageBox.Show($"{unit.UnitName} cannot be dispatched while status is {unit.Status}.", "Dispatch Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Dispatch {unit.UnitName} now?",
                "Confirm Dispatch",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            unit.Status = "Dispatched";
            RefreshDataGrid();
            MessageBox.Show($"{unit.UnitName} has been dispatched.", "Dispatch Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // RETURN TO BASE
        private void button4_Click(object? sender, EventArgs e)
        {
            if (GlobalUnitList.Count == 0)
            {
                MessageBox.Show("There are no units to return.", "Return to Base", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var unit = GetSelectedUnit();
            if (unit == null)
            {
                return;
            }

            if (unit.Status != "Dispatched")
            {
                MessageBox.Show($"{unit.UnitName} is not dispatched.", "Return to Base", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            unit.Status = "Available";
            unit.Location = "Base";
            RefreshDataGrid();
            MessageBox.Show($"{unit.UnitName} returned to base and is now Available.", "Return to Base", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // REMOVE UNITS
        private void button5_Click(object? sender, EventArgs e)
        {
            if (GlobalUnitList.Count == 0)
            {
                MessageBox.Show("There are no units to remove yet.", "Remove Units", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var unit = GetSelectedUnit();
            if (unit == null)
            {
                return;
            }

            var confirm = MessageBox.Show(
                $"Remove {unit.UnitName} from the system?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                GlobalUnitList.Remove(unit);
                RenumberUnitIds();
                RefreshDataGrid();
            }
        }

        // LOGOUT
        private void button6_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }

    // This class handles the data structure for your units
    public class ResponseUnit
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
