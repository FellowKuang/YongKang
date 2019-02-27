namespace 中车
{
    partial class cyzdjlkz
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.btnStart = new System.Windows.Forms.Button();
            this.chartDemo = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnContine = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.cyzdjl_chartTimer = new System.Windows.Forms.Timer(this.components);
            this.cyzdjl_controlPort = new System.IO.Ports.SerialPort(this.components);
            this.cbZdLevel = new System.Windows.Forms.ComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.chartDemo)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(302, 1006);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(116, 41);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // chartDemo
            // 
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.chartDemo.ChartAreas.Add(chartArea1);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.chartDemo.Legends.Add(legend1);
            this.chartDemo.Location = new System.Drawing.Point(12, 35);
            this.chartDemo.Name = "chartDemo";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Legend = "Legend1";
            series4.Name = "Series4";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series5.Legend = "Legend1";
            series5.Name = "Series5";
            this.chartDemo.Series.Add(series1);
            this.chartDemo.Series.Add(series2);
            this.chartDemo.Series.Add(series3);
            this.chartDemo.Series.Add(series4);
            this.chartDemo.Series.Add(series5);
            this.chartDemo.Size = new System.Drawing.Size(1605, 948);
            this.chartDemo.TabIndex = 1;
            this.chartDemo.Text = "chart1";
            this.chartDemo.Click += new System.EventHandler(this.chartDemo_Click);
            this.chartDemo.DoubleClick += new System.EventHandler(this.chartDemo_DoubleClick);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(654, 1005);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(116, 41);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "暂停";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnContine
            // 
            this.btnContine.Location = new System.Drawing.Point(935, 1002);
            this.btnContine.Name = "btnContine";
            this.btnContine.Size = new System.Drawing.Size(116, 41);
            this.btnContine.TabIndex = 3;
            this.btnContine.Text = "继续";
            this.btnContine.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1232, 1005);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(116, 41);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // cyzdjl_chartTimer
            // 
            this.cyzdjl_chartTimer.Tick += new System.EventHandler(this.cyzdjl_chartTimer_Tick);
            // 
            // cbZdLevel
            // 
            this.cbZdLevel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbZdLevel.FormattingEnabled = true;
            this.cbZdLevel.Items.AddRange(new object[] {
            "一级制动",
            "二级制动",
            "三级制动",
            "四级制动",
            "五级制动",
            "六级制动",
            "七级制动"});
            this.cbZdLevel.Location = new System.Drawing.Point(42, 1007);
            this.cbZdLevel.Name = "cbZdLevel";
            this.cbZdLevel.Size = new System.Drawing.Size(179, 32);
            this.cbZdLevel.TabIndex = 5;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1629, 27);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(128, 24);
            this.toolStripLabel1.Text = "串口设置（&S）";
            this.toolStripLabel1.Click += new System.EventHandler(this.toolStripLabel1_Click);
            // 
            // cyzdjlkz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1629, 1050);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cbZdLevel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnContine);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.chartDemo);
            this.Controls.Add(this.btnStart);
            this.Name = "cyzdjlkz";
            this.Text = "cyzdjlkz";
            this.Load += new System.EventHandler(this.cyzdjlkz_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartDemo)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDemo;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnContine;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer cyzdjl_chartTimer;
        private System.IO.Ports.SerialPort cyzdjl_controlPort;
        private System.Windows.Forms.ComboBox cbZdLevel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}