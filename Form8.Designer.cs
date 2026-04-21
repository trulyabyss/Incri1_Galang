namespace Incri1_Galang
{
    partial class Form8
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
            textBox1 = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label1 = new Label();
            label2 = new Label();
            dateTimePicker1 = new DateTimePicker();
            label3 = new Label();
            label6 = new Label();
            label7 = new Label();
            textBox2 = new TextBox();
            Button1_Click = new Button();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            label8 = new Label();
            textBox3 = new TextBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(165, 234);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Engine 5";
            textBox1.Size = new Size(201, 23);
            textBox1.TabIndex = 0;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label5.Location = new Point(161, 74);
            label5.Name = "label5";
            label5.Size = new Size(89, 23);
            label5.TabIndex = 11;
            label5.Text = "SYSTEM";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label4.Location = new Point(56, 51);
            label4.Name = "label4";
            label4.Size = new Size(285, 23);
            label4.TabIndex = 10;
            label4.Text = "DISASTER TRUCK DISPATCH";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.Control;
            label1.Font = new Font("Rockwell", 20F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(56, 105);
            label1.Name = "label1";
            label1.Size = new Size(306, 33);
            label1.TabIndex = 12;
            label1.Text = "ADD RESPONSE UNIT";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 242);
            label2.Name = "label2";
            label2.Size = new Size(110, 15);
            label2.TabIndex = 13;
            label2.Text = "ENTER UNIT NAME:";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(163, 157);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(201, 23);
            dateTimePicker1.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(64, 165);
            label3.Name = "label3";
            label3.Size = new Size(69, 15);
            label3.TabIndex = 15;
            label3.Text = "DATE & TIME:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(39, 204);
            label6.Name = "label6";
            label6.Size = new Size(101, 15);
            label6.TabIndex = 16;
            label6.Text = "UNIT BELONG TO:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(73, 280);
            label7.Name = "label7";
            label7.Size = new Size(69, 15);
            label7.TabIndex = 19;
            label7.Text = "STATIONED:";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(163, 272);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "ex. Pardo Substation";
            textBox2.Size = new Size(201, 23);
            textBox2.TabIndex = 18;
            // 
            // Button1_Click
            // 
            Button1_Click.Location = new Point(287, 357);
            Button1_Click.Name = "Button1_Click";
            Button1_Click.Size = new Size(75, 23);
            Button1_Click.TabIndex = 20;
            Button1_Click.Text = "Save";
            Button1_Click.UseVisualStyleBackColor = true;
            Button1_Click.Click += button1_Click;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(165, 202);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(45, 19);
            radioButton1.TabIndex = 21;
            radioButton1.TabStop = true;
            radioButton1.Text = "BFP";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(227, 202);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(48, 19);
            radioButton2.TabIndex = 22;
            radioButton2.TabStop = true;
            radioButton2.Text = "PNP";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(293, 202);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(48, 19);
            radioButton3.TabIndex = 23;
            radioButton3.TabStop = true;
            radioButton3.Text = "EMS";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(67, 313);
            label8.Name = "label8";
            label8.Size = new Size(73, 15);
            label8.TabIndex = 24;
            label8.Text = "RESOURCES:";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(165, 305);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "ex. Hazmat Team";
            textBox3.Size = new Size(201, 23);
            textBox3.TabIndex = 25;
            // 
            // Form8
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(393, 450);
            Controls.Add(textBox3);
            Controls.Add(label8);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(Button1_Click);
            Controls.Add(label7);
            Controls.Add(textBox2);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(dateTimePicker1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(textBox1);
            Name = "Form8";
            Text = "Form8";
            Load += Form8_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label5;
        private Label label4;
        private Label label1;
        private Label label2;
        private DateTimePicker dateTimePicker1;
        private Label label3;
        private Label label6;
        private Label label7;
        private TextBox textBox2;
        private Button Button1_Click;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private Label label8;
        private TextBox textBox3;
    }
}