using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Incri1_Galang
{
    public class ReportsAnalyticsForm : Form
    {
        private readonly Label _totalValueLabel;
        private readonly Label _topTypeValueLabel;
        private readonly Label _topBarangayValueLabel;
        private readonly Chart _typeChart;
        private readonly Label _emptyChartLabel;
        private readonly DataGridView _barangayGrid;

        public ReportsAnalyticsForm()
        {
            Text = "Reports Analytics";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 600);
            MinimumSize = new Size(900, 520);

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            Controls.Add(mainLayout);

            Panel headerPanel = new Panel { Dock = DockStyle.Fill };
            Label titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Rockwell", 16F, FontStyle.Bold),
                Location = new Point(0, 0),
                Text = "REPORTS ANALYTICS"
            };
            Label subtitleLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(2, 32),
                Text = "Based on resident incident reports"
            };
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            mainLayout.Controls.Add(headerPanel, 0, 0);

            TableLayoutPanel statsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            Panel totalPanel = CreateStatCard("Total Reports", out _totalValueLabel);
            Panel typePanel = CreateStatCard("Top Incident Type", out _topTypeValueLabel);
            Panel barangayPanel = CreateStatCard("Top Barangay", out _topBarangayValueLabel);

            statsLayout.Controls.Add(totalPanel, 0, 0);
            statsLayout.Controls.Add(typePanel, 1, 0);
            statsLayout.Controls.Add(barangayPanel, 2, 0);
            mainLayout.Controls.Add(statsLayout, 0, 1);

            SplitContainer contentSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 6,
                Panel1MinSize = 200,
                Panel2MinSize = 150
            };
            mainLayout.Controls.Add(contentSplit, 0, 2);

            Panel chartPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6) };
            Label chartLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Text = "Reports by Type"
            };
            _typeChart = new Chart { Dock = DockStyle.Fill };
            ChartArea chartArea = new ChartArea("Main")
            {
                AxisX =
                {
                    Interval = 1,
                    MajorGrid = { Enabled = false },
                    LabelStyle = { Angle = -20 }
                },
                AxisY =
                {
                    Minimum = 0,
                    MajorGrid = { LineColor = Color.Gainsboro }
                }
            };
            _typeChart.ChartAreas.Add(chartArea);
            _typeChart.Legends.Clear();
            _typeChart.Palette = ChartColorPalette.Bright;

            _emptyChartLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "No report data available",
                ForeColor = Color.DimGray,
                Visible = false
            };

            chartPanel.Controls.Add(_typeChart);
            chartPanel.Controls.Add(_emptyChartLabel);
            chartPanel.Controls.Add(chartLabel);
            contentSplit.Panel1.Controls.Add(chartPanel);

            Panel gridPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6) };
            Label gridLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Text = "Reports by Barangay"
            };

            _barangayGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            gridPanel.Controls.Add(_barangayGrid);
            gridPanel.Controls.Add(gridLabel);
            contentSplit.Panel2.Controls.Add(gridPanel);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            Button closeButton = new Button
            {
                Text = "CLOSE",
                Size = new Size(90, 28)
            };
            closeButton.Click += (_, _) => Close();

            Button refreshButton = new Button
            {
                Text = "REFRESH",
                Size = new Size(90, 28)
            };
            refreshButton.Click += (_, _) => LoadAnalytics();

            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(refreshButton);
            mainLayout.Controls.Add(buttonPanel, 0, 3);

            UnitDatabase.DataChanged += HandleDataChanged;
            FormClosed += HandleFormClosed;

            LoadAnalytics();
        }

        private void HandleDataChanged(object? sender, EventArgs e)
        {
            LoadAnalytics();
        }

        private void HandleFormClosed(object? sender, FormClosedEventArgs e)
        {
            UnitDatabase.DataChanged -= HandleDataChanged;
            FormClosed -= HandleFormClosed;
        }

        private void LoadAnalytics()
        {
            UnitDatabase.Initialize();
            List<ResponseUnit> reports = UnitDatabase.GetAllReports();

            _totalValueLabel.Text = reports.Count.ToString("N0");

            List<NamedCount> byType = reports
                .GroupBy(unit => NormalizeKey(unit.UnitType))
                .Select(group => new NamedCount(group.Key, group.Count()))
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name)
                .ToList();

            List<NamedCount> byBarangay = reports
                .GroupBy(unit => NormalizeKey(unit.Location))
                .Select(group => new NamedCount(group.Key, group.Count()))
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Name)
                .ToList();

            NamedCount? topType = byType.FirstOrDefault();
            _topTypeValueLabel.Text = topType == null
                ? "N/A"
                : $"{topType.Name} ({topType.Count})";

            NamedCount? topBarangay = byBarangay.FirstOrDefault();
            _topBarangayValueLabel.Text = topBarangay == null
                ? "N/A"
                : $"{topBarangay.Name} ({topBarangay.Count})";

            UpdateChart(byType);
            UpdateBarangayGrid(byBarangay);
        }

        private void UpdateChart(List<NamedCount> byType)
        {
            _typeChart.Series.Clear();

            Series series = new Series("Reports")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            foreach (NamedCount item in byType)
            {
                series.Points.AddXY(item.Name, item.Count);
            }

            _typeChart.Series.Add(series);
            _emptyChartLabel.Visible = byType.Count == 0;
        }

        private void UpdateBarangayGrid(List<NamedCount> byBarangay)
        {
            List<BarangayRow> rows = byBarangay
                .Select(item => new BarangayRow { Barangay = item.Name, Reports = item.Count })
                .ToList();

            _barangayGrid.DataSource = null;
            _barangayGrid.DataSource = rows;

            if (_barangayGrid.Columns["Barangay"] is DataGridViewColumn barangayColumn)
            {
                barangayColumn.HeaderText = "Barangay";
                barangayColumn.DisplayIndex = 0;
            }

            if (_barangayGrid.Columns["Reports"] is DataGridViewColumn reportsColumn)
            {
                reportsColumn.HeaderText = "Reports";
                reportsColumn.DisplayIndex = 1;
            }
        }

        private static string NormalizeKey(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value.Trim();
        }

        private static Panel CreateStatCard(string title, out Label valueLabel)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10, 8, 10, 6),
                Margin = new Padding(4)
            };

            valueLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(0, 0),
                Text = "0"
            };

            Label titleLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(2, 34),
                Text = title
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(titleLabel);
            return panel;
        }

        private sealed class NamedCount
        {
            public NamedCount(string name, int count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; }
            public int Count { get; }
        }

        private sealed class BarangayRow
        {
            public string Barangay { get; set; } = string.Empty;
            public int Reports { get; set; }
        }
    }
}
