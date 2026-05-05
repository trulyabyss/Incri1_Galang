using System;
using System.Windows.Forms;

namespace Incri1_Galang
{
    public partial class Form14 : Form
    {
        public string NotesText => textBox1.Text.Trim();

        public Form14(string initialNotes)
        {
            InitializeComponent();

            textBox1.Text = initialNotes ?? string.Empty;
            button1.Click += HandleSave;
            button2.Click += HandleCancel;
        }

        private void HandleSave(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void HandleCancel(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
