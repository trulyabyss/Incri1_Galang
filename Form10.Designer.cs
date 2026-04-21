namespace Incri1_Galang
{
    partial class Form10
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label4 = new Label();
            label1 = new Label();
            textBox2 = new TextBox();
            label2 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = false;
            label4.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label4.Location = new Point(16, 24);
            label4.Name = "label4";
            label4.Size = new Size(270, 46);
            label4.TabIndex = 12;
            label4.Text = "DISPATCH UNIT";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(36, 77);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 14;
            label1.Text = "Enter Disaster Location:";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(36, 95);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(189, 23);
            textBox2.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(36, 132);
            label2.Name = "label2";
            label2.Size = new Size(119, 15);
            label2.TabIndex = 16;
            label2.Text = "Description of Alarm:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(36, 150);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(189, 23);
            textBox1.TabIndex = 17;
            // 
            // button1
            // 
            button1.Location = new Point(215, 179);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 22;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form10
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(302, 211);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(label1);
            Controls.Add(label4);
            Name = "Form10";
            Text = "Form10";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
        private Label label1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox1;
        private Button button1;
    }
}