using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            button1.Click += HandleFireReport;
            button2.Click += HandlePoliceReport;
            button4.Click += HandleMedicalReport;
            button3.Click += HandleUndo;
            button11.Click += HandleOpenChat;
        }

        private void HandleFireReport(object? sender, EventArgs e)
        {
            using (Form4 fireForm = new Form4())
            {
                fireForm.ShowDialog(this);
            }
        }

        private void HandlePoliceReport(object? sender, EventArgs e)
        {
            using (Form5 policeForm = new Form5())
            {
                policeForm.ShowDialog(this);
            }
        }

        private void HandleMedicalReport(object? sender, EventArgs e)
        {
            using (Form6 medicalForm = new Form6())
            {
                medicalForm.ShowDialog(this);
            }
        }

        private void HandleUndo(object? sender, EventArgs e)
        {
            Close();
        }

        private void HandleOpenChat(object? sender, EventArgs e)
        {
            using ChatForm chatForm = new ChatForm(ChatRole.Resident);
            chatForm.ShowDialog(this);
        }
    }
}
