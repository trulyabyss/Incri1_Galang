using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string unitName = textBox1.Text.Trim();
            string location = textBox2.Text.Trim();
            string resources = textBox3.Text.Trim();

            string type = radioButton1.Checked
                ? "BFP"
                : radioButton2.Checked
                    ? "PNP"
                    : radioButton3.Checked
                        ? "EMS"
                        : string.Empty;

            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Please select Unit Belong To (BFP, PNP, or EMS).", "Missing Unit Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(unitName))
            {
                MessageBox.Show("Please enter a Unit Name.", "Missing Unit Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter where the unit is stationed.", "Missing Station", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            ResponseUnit newUnit = new ResponseUnit
            {
                UnitName = unitName,
                UnitType = type,
                Status = "Available",
                Location = location,
                DateAdded = dateTimePicker1.Value,
                Resources = resources
            };

            try
            {
                newUnit.UnitID = UnitDatabase.AddUnit(newUnit);
                Form7.GlobalUnitList.Add(newUnit);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save unit to database.\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}