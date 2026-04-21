using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form10 : Form
    {
        private readonly ResponseUnit _unit;

        public Form10(ResponseUnit unit)
        {
            InitializeComponent();
            _unit = unit;
            label4.Text = $"DISPATCH UNIT: ID {_unit.UnitID} ({_unit.UnitName})";
            textBox2.Text = _unit.IncidentLocation;
            textBox1.Text = _unit.AlarmDescription;
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            string incidentLocation = textBox2.Text.Trim();
            string alarmDescription = textBox1.Text.Trim();

            if (!string.Equals(_unit.Status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Only units with status 'Available' can be dispatched.", "Dispatch Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(incidentLocation))
            {
                MessageBox.Show("Please enter the incident location.", "Missing Incident Location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            _unit.IncidentLocation = incidentLocation;
            _unit.AlarmDescription = alarmDescription;
            _unit.Status = "Unavailable";

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
