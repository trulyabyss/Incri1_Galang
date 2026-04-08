using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
            button1.Click += button1_Click;
        }

        private void Form8_Load(object? sender, EventArgs e)
        {
            // Set default date/time to now when the form opens
            dateTimePicker1.Value = DateTime.Now;
        }

        // button1: SAVE Button
        private void button1_Click(object? sender, EventArgs e)
        {
            // 1. Capture the Unit Name, Station, and Remark from TextBoxes
            string unitName = textBox1.Text.Trim();
            string stationedAt = textBox2.Text.Trim();
            string remark = textBoxRemark.Text.Trim();

            // 2. Capture the Unit Type from RadioButtons
            string unitType = "";
            if (radioButton1.Checked) unitType = "BFP";
            else if (radioButton2.Checked) unitType = "PNP";
            else if (radioButton3.Checked) unitType = "EMS";

            // 3. Validation Logic
            if (string.IsNullOrEmpty(unitName))
            {
                MessageBox.Show("Please enter a Unit Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(unitType))
            {
                MessageBox.Show("Please select which organization the unit belongs to (BFP, PNP, or EMS).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Create the new Unit Object
            // We use Form7.GlobalUnitList to access the central data
            ResponseUnit newUnit = new ResponseUnit
            {
                UnitID = Form7.GlobalUnitList.Count + 1, // Simple auto-increment
                UnitName = unitName,
                UnitType = unitType,
                Location = stationedAt,
                Status = "Available", // Default status for new units
                Remark = remark
            };

            // 5. Save to the Global List
            Form7.GlobalUnitList.Add(newUnit);

            // 6. Success Feedback and Close
            MessageBox.Show($"{unitName} successfully registered to the {unitType} fleet.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close(); // Return to Form 7
        }
    }
}
