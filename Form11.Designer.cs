namespace Incri1_Galang
{
    partial class Form11
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
            panelTop = new Panel();
            lblRouteInfo = new Label();
            txtIncidentLocation = new TextBox();
            label2 = new Label();
            comboUnits = new ComboBox();
            label1 = new Label();
            mapView = new Microsoft.Web.WebView2.WinForms.WebView2();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mapView).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = SystemColors.ControlLight;
            panelTop.Controls.Add(lblRouteInfo);
            panelTop.Controls.Add(txtIncidentLocation);
            panelTop.Controls.Add(label2);
            panelTop.Controls.Add(comboUnits);
            panelTop.Controls.Add(label1);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1000, 93);
            panelTop.TabIndex = 0;
            // 
            // lblRouteInfo
            // 
            lblRouteInfo.AutoSize = true;
            lblRouteInfo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRouteInfo.Location = new Point(18, 66);
            lblRouteInfo.Name = "lblRouteInfo";
            lblRouteInfo.Size = new Size(150, 15);
            lblRouteInfo.TabIndex = 5;
            lblRouteInfo.Text = "Enter a landmark to pin on the map.";
            // 
            // txtIncidentLocation
            // 
            txtIncidentLocation.Location = new Point(494, 28);
            txtIncidentLocation.Name = "txtIncidentLocation";
            txtIncidentLocation.PlaceholderText = "e.g. Colon St, Cebu City";
            txtIncidentLocation.Size = new Size(344, 23);
            txtIncidentLocation.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(390, 31);
            label2.Name = "label2";
            label2.Size = new Size(93, 15);
            label2.TabIndex = 2;
            label2.Text = "Incident Area:";
            // 
            // comboUnits
            // 
            comboUnits.DropDownStyle = ComboBoxStyle.DropDownList;
            comboUnits.FormattingEnabled = true;
            comboUnits.Location = new Point(99, 28);
            comboUnits.Name = "comboUnits";
            comboUnits.Size = new Size(271, 23);
            comboUnits.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 31);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 0;
            label1.Text = "Select Unit:";
            // 
            // mapView
            // 
            mapView.AllowExternalDrop = false;
            mapView.CreationProperties = null;
            mapView.DefaultBackgroundColor = Color.White;
            mapView.Dock = DockStyle.Fill;
            mapView.Location = new Point(0, 93);
            mapView.Name = "mapView";
            mapView.Size = new Size(1000, 557);
            mapView.TabIndex = 1;
            mapView.ZoomFactor = 1D;
            // 
            // Form11
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 650);
            Controls.Add(mapView);
            Controls.Add(panelTop);
            Name = "Form11";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Map Operations";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mapView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTop;
        private Label lblRouteInfo;
        private TextBox txtIncidentLocation;
        private Label label2;
        private ComboBox comboUnits;
        private Label label1;
        private Microsoft.Web.WebView2.WinForms.WebView2 mapView;
    }
}