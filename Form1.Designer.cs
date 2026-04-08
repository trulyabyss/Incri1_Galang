namespace Incri1_Galang
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            button1 = new Button();
            pictureBox1 = new PictureBox();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.Control;
            label1.Font = new Font("Rockwell", 20F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(148, 169);
            label1.Name = "label1";
            label1.Size = new Size(105, 33);
            label1.TabIndex = 0;
            label1.Text = "LOGIN";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(125, 230);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(198, 23);
            textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(125, 273);
            textBox2.Name = "textBox2";
            textBox2.PasswordChar = '*';
            textBox2.Size = new Size(198, 23);
            textBox2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(50, 233);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 3;
            label2.Text = "Username : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(53, 276);
            label3.Name = "label3";
            label3.Size = new Size(66, 15);
            label3.TabIndex = 4;
            label3.Text = "Password : ";
            // 
            // button1
            // 
            button1.Location = new Point(230, 312);
            button1.Name = "button1";
            button1.Size = new Size(93, 31);
            button1.TabIndex = 5;
            button1.Text = "LOGIN";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(114, 82);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(174, 84);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label4.Location = new Point(50, 19);
            label4.Name = "label4";
            label4.Size = new Size(285, 23);
            label4.TabIndex = 7;
            label4.Text = "DISASTER TRUCK DISPATCH";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label5.Location = new Point(155, 42);
            label5.Name = "label5";
            label5.Size = new Size(89, 23);
            label5.TabIndex = 8;
            label5.Text = "SYSTEM";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(393, 375);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(pictureBox1);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            MaximizeBox = false;
            Name = "Form1";
            Text = "Disaster Truck Dispatch System";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label2;
        private Label label3;
        private Button button1;
        private PictureBox pictureBox1;
        private Label label4;
        private Label label5;
    }
}
