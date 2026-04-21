using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Mask the password in the password box (textBox1)
            textBox1.UseSystemPasswordChar = true;

            // Wire up the button events
            button1.Click += btnEnter_Click;
            button2.Click += btnRegister_Click;
        }

        private void btnEnter_Click(object? sender, EventArgs e)
        {
            // textBox2 is Username, textBox1 is Password based on your designer
            string user = textBox2.Text.Trim();
            string pass = textBox1.Text;

            // 1. Validation: Ensure fields aren't empty
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter your credentials.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Admin Logic: Check for admin credentials
            if (user.Equals("admin", StringComparison.OrdinalIgnoreCase) && pass == "admin123") // Replace "admin123" with your desired password
            {
                MessageBox.Show("Admin Access Granted. Opening System Dashboard...", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open dashboard as a modal session; once it closes, return to login.
                this.Hide();
                using (Form7 adminDashboard = new Form7())
                {
                    adminDashboard.ShowDialog(this);
                }

                textBox1.Clear();
                this.Show();
                this.Activate();
            }
            else
            {
                // Logic for failed login
                MessageBox.Show("Invalid Admin credentials. Please try again.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear(); // Clear password for security
            }
        }

        // --- THIS IS THE FIX TO CONNECT TO FORM2 ---
        private void btnRegister_Click(object? sender, EventArgs e)
        {
            // 1. Create an instance of Form2
            Form2 registrationForm = new Form2();

            // 2. Hide Form1 so it's not in the way
            this.Hide();

            // 3. Show Form2. Use .ShowDialog() so the code waits 
            // until Form2 is closed before continuing.
            registrationForm.ShowDialog();

            // 4. After Form2 is closed, show Form1 again so they can log in
            this.Show();
        }
    }
}