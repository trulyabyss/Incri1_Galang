using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            // Set the password mask for security
            textBox1.UseSystemPasswordChar = true;

            // Wire up the Register button click event
            button1.Click += btnRegisterAccount_Click;
        }

        private void btnRegisterAccount_Click(object? sender, EventArgs e)
        {
            // Capture data from designer textboxes
            string newUsername = textBox2.Text.Trim(); // Top box
            string newPassword = textBox1.Text;        // Bottom box

            // 1. Validation Logic
            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Registration failed. Please provide both a username and a password.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Logic for minimum password length (Optional but recommended)
            if (newPassword.Length < 6)
            {
                MessageBox.Show("For security, your password must be at least 6 characters long.",
                                "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. "Saving" Logic 
            // In a real app, you would send 'newUsername' and 'newPassword' to a Database here.
            MessageBox.Show($"Account for '{newUsername}' has been successfully created!",
                            "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 4. Return to Login
            // This closes the registration form and takes the user back to Form1
            this.Close();
        }
    }
}