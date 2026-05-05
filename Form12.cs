using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form12 : Form
    {
        public Form12()
        {
            InitializeComponent();

            textBox2.UseSystemPasswordChar = true;
            button1.Click += HandleLogin;
        }

        private void HandleLogin(object? sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            try
            {
                BarangayUserDatabase.Initialize();
                if (!BarangayUserDatabase.TryGetBarangayForUser(username, password, out string barangayName, out string error))
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        MessageBox.Show(error, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MessageBox.Show("Invalid barangay credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Hide();
                using (Form15 barangayDashboard = new Form15())
                {
                    barangayDashboard.ShowDialog(this);
                }
                Show();
                Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed. " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
