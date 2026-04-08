using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            // textBox1 is Username, textBox2 is Password based on your designer
            string user = textBox1.Text.Trim();
            string pass = textBox2.Text;

            // 1. Validation: Ensure fields aren't empty
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter your credentials.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Admin Logic: Check for admin credentials
            if (user.ToLower() == "admin" && pass == "admin123")
            {
                MessageBox.Show("Admin Access Granted. Opening System Dashboard...", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Create an instance of Form7
                Form7 adminDashboard = new Form7();

                // Hide login while admin dashboard is open
                this.Hide();

                // Show as dialog so when admin logs out/closes dashboard,
                // execution returns here and login is shown again.
                adminDashboard.ShowDialog();

                this.Show();
                textBox2.Clear();
                textBox1.Focus();
            }
            else
            {
                // Logic for failed login
                MessageBox.Show("Invalid Admin credentials. Please try again.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear(); // Clear password for security
            }
        }
    }
}
