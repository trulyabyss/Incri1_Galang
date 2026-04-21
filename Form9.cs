using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form9 : Form
    {
        private ResponseUnit _unit;

        // Ensure the constructor accepts a ResponseUnit
        public Form9(ResponseUnit unit)
        {
            InitializeComponent();
            _unit = unit;
            label4.Text = $"Unit ID {_unit.UnitID}: {_unit.UnitName}";

            int statusIndex = comboBox1.FindStringExact(_unit.Status);
            if (statusIndex >= 0)
            {
                comboBox1.SelectedIndex = statusIndex;
            }
            else
            {
                comboBox1.Text = _unit.Status;
            }
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            string selectedStatus = comboBox1.SelectedItem?.ToString() ?? comboBox1.Text.Trim();

            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                _unit.Status = selectedStatus;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            MessageBox.Show("Please select a unit status before saving.", "Missing Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}