using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form16 : Form
    {
        private readonly ResponseUnit _unit;

        public Form16(ResponseUnit unit)
        {
            InitializeComponent();

            _unit = unit;
            label4.Text = $"UPDATE INCIDENT STATUS: ID {_unit.UnitID}";

            int statusIndex = comboBox1.FindStringExact(_unit.Status);
            if (statusIndex >= 0)
            {
                comboBox1.SelectedIndex = statusIndex;
            }
            else
            {
                comboBox1.Text = _unit.Status;
            }

            button1.Click += HandleSave;
        }

        private void HandleSave(object? sender, EventArgs e)
        {
            string selectedStatus = comboBox1.SelectedItem?.ToString() ?? comboBox1.Text.Trim();
            if (!string.IsNullOrWhiteSpace(selectedStatus))
            {
                _unit.Status = selectedStatus;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show("Please select a status before saving.", "Missing Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
