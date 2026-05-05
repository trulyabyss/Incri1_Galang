using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();

            button1.Click += HandleSubmit;
        }

        private void HandleSubmit(object? sender, EventArgs e)
        {
            string location = textBox1.Text.Trim();
            string status = textBox4.Text.Trim();
            string incidentNature = textBox5.Text.Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter the place of incident.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string trapped = radioButton4.Checked ? "Yes" : radioButton3.Checked ? "No" : "Unknown";
            string hazardous = radioButton1.Checked ? "Yes" : radioButton2.Checked ? "No" : "Unknown";

            string alarmDescription =
                $"Status: {status}; Nature: {incidentNature}; Trapped: {trapped}; Hazardous Chemicals: {hazardous}.";

            ResponseUnit report = new ResponseUnit
            {
                UnitName = "RESIDENT MEDICAL REPORT",
                UnitType = "Medical",
                Status = "Reported",
                Location = location,
                DateAdded = dateTimePicker1.Value,
                Resources = incidentNature,
                IncidentLocation = location,
                AlarmDescription = alarmDescription
            };

            try
            {
                UnitDatabase.Initialize();
                report.UnitID = UnitDatabase.AddReport(report);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save your medical report. " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Your medical report has been submitted.",
                "Report Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
