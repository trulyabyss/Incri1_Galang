using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form17 : Form
    {
        public Form17()
        {
            InitializeComponent();

            textBox1.UseSystemPasswordChar = true;
            button1.Click += btnRegisterAccount_Click;
        }

        private void btnRegisterAccount_Click(object? sender, EventArgs e)
        {
            string newUsername = textBox2.Text.Trim();
            string newPassword = textBox1.Text;
            string barangayName = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Registration failed. Please provide both a username and a password.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("For security, your password must be at least 6 characters long.",
                    "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                BarangayUserDatabase.Initialize();
                if (!BarangayUserDatabase.RegisterBarangayUser(newUsername, newPassword, barangayName, out string error))
                {
                    MessageBox.Show(error, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed. " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Account for '{newUsername}' has been successfully created!",
                "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string databasePath = UnitDatabase.GetResolvedDatabasePath();
            MessageBox.Show("Saved in: " + databasePath,
                "Database Location", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}
