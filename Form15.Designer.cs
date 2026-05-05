namespace Incri1_Galang
{
    partial class Form15
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
            dataGridView1 = new DataGridView();
            label1 = new Label();
            label5 = new Label();
            label4 = new Label();
            searchLabel = new Label();
            searchBox = new TextBox();
            clearButton = new Button();
            button6 = new Button();
            button3 = new Button();
            groupBoxRemoveOptions = new GroupBox();
            radioRemoveViewOnly = new RadioButton();
            radioRemoveDatabase = new RadioButton();
            button2 = new Button();
            button10 = new Button();
            button11 = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBoxRemoveOptions.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(201, 124);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(490, 287);
            dataGridView1.TabIndex = 13;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.Control;
            label1.Font = new Font("Rockwell", 20F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(242, 61);
            label1.Name = "label1";
            label1.Size = new Size(266, 33);
            label1.TabIndex = 16;
            label1.Text = "BARANGAY MENU";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label5.Location = new Point(328, 38);
            label5.Name = "label5";
            label5.Size = new Size(89, 23);
            label5.TabIndex = 15;
            label5.Text = "SYSTEM";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Rockwell", 14F, FontStyle.Bold);
            label4.Location = new Point(265, 15);
            label4.Name = "label4";
            label4.Size = new Size(210, 23);
            label4.TabIndex = 14;
            label4.Text = "DISASTER DISPATCH";
            // 
            // searchLabel
            // 
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(363, 100);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(45, 15);
            searchLabel.TabIndex = 38;
            searchLabel.Text = "Search:";
            // 
            // searchBox
            // 
            searchBox.Location = new Point(414, 97);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(200, 23);
            searchBox.TabIndex = 39;
            // 
            // clearButton
            // 
            clearButton.Location = new Point(620, 95);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(70, 23);
            clearButton.TabIndex = 40;
            clearButton.Text = "CLEAR";
            clearButton.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(600, 417);
            button6.Name = "button6";
            button6.Size = new Size(90, 23);
            button6.TabIndex = 33;
            button6.Text = "LOGOUT";
            button6.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.BackColor = Color.Aquamarine;
            button3.Location = new Point(5, 258);
            button3.Name = "button3";
            button3.Size = new Size(190, 54);
            button3.TabIndex = 31;
            button3.Text = "REMOVE";
            button3.UseVisualStyleBackColor = false;
            // 
            // groupBoxRemoveOptions
            // 
            groupBoxRemoveOptions.Controls.Add(radioRemoveViewOnly);
            groupBoxRemoveOptions.Controls.Add(radioRemoveDatabase);
            groupBoxRemoveOptions.Location = new Point(11, 316);
            groupBoxRemoveOptions.Name = "groupBoxRemoveOptions";
            groupBoxRemoveOptions.Size = new Size(190, 50);
            groupBoxRemoveOptions.TabIndex = 32;
            groupBoxRemoveOptions.TabStop = false;
            groupBoxRemoveOptions.Text = "Remove Options";
            // 
            // radioRemoveViewOnly
            // 
            radioRemoveViewOnly.AutoSize = true;
            radioRemoveViewOnly.Location = new Point(108, 21);
            radioRemoveViewOnly.Name = "radioRemoveViewOnly";
            radioRemoveViewOnly.Size = new Size(76, 19);
            radioRemoveViewOnly.TabIndex = 1;
            radioRemoveViewOnly.Text = "View only";
            radioRemoveViewOnly.UseVisualStyleBackColor = true;
            // 
            // radioRemoveDatabase
            // 
            radioRemoveDatabase.AutoSize = true;
            radioRemoveDatabase.Checked = true;
            radioRemoveDatabase.Location = new Point(10, 21);
            radioRemoveDatabase.Name = "radioRemoveDatabase";
            radioRemoveDatabase.Size = new Size(73, 19);
            radioRemoveDatabase.TabIndex = 0;
            radioRemoveDatabase.TabStop = true;
            radioRemoveDatabase.Text = "Database";
            radioRemoveDatabase.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.BackColor = Color.Thistle;
            button2.Location = new Point(5, 124);
            button2.Name = "button2";
            button2.Size = new Size(190, 64);
            button2.TabIndex = 30;
            button2.Text = "REPORT STATUS";
            button2.UseVisualStyleBackColor = false;
            // 
            // button10
            // 
            button10.BackColor = Color.PaleGreen;
            button10.Location = new Point(5, 372);
            button10.Name = "button10";
            button10.Size = new Size(190, 23);
            button10.TabIndex = 36;
            button10.Text = "VIEW USERS";
            button10.UseVisualStyleBackColor = false;
            // 
            // button11
            // 
            button11.Location = new Point(57, 411);
            button11.Name = "button11";
            button11.Size = new Size(90, 23);
            button11.TabIndex = 37;
            button11.Text = "OPEN CHAT";
            button11.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.BackColor = Color.SandyBrown;
            button1.Location = new Point(5, 194);
            button1.Name = "button1";
            button1.Size = new Size(190, 58);
            button1.TabIndex = 35;
            button1.Text = "SHOW DATA";
            button1.UseVisualStyleBackColor = false;
            // 
            // Form15
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(702, 443);
            Controls.Add(clearButton);
            Controls.Add(searchBox);
            Controls.Add(searchLabel);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button1);
            Controls.Add(button6);
            Controls.Add(button3);
            Controls.Add(groupBoxRemoveOptions);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(dataGridView1);
            Name = "Form15";
            Text = "Form15";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBoxRemoveOptions.ResumeLayout(false);
            groupBoxRemoveOptions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private Label label5;
        private Label label4;
        private Label searchLabel;
        private TextBox searchBox;
        private Button clearButton;
        private Button button6;
        private Button button3;
        private GroupBox groupBoxRemoveOptions;
        private RadioButton radioRemoveDatabase;
        private RadioButton radioRemoveViewOnly;
        private Button button2;
        private Button button10;
        private Button button11;
        private Button button1;
    }
}