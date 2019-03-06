namespace 中车
{
    partial class jlSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clbBianzu = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clbTrain = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSure = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clbBianzu);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 380);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "列车最大编组数量";
            // 
            // clbBianzu
            // 
            this.clbBianzu.FormattingEnabled = true;
            this.clbBianzu.Items.AddRange(new object[] {
            "单车编组",
            "80辆车编组",
            "100辆车编组",
            "150辆车编组"});
            this.clbBianzu.Location = new System.Drawing.Point(69, 24);
            this.clbBianzu.Name = "clbBianzu";
            this.clbBianzu.Size = new System.Drawing.Size(161, 324);
            this.clbBianzu.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.clbTrain);
            this.groupBox2.Location = new System.Drawing.Point(350, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 380);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "级联列车数量";
            // 
            // clbTrain
            // 
            this.clbTrain.FormattingEnabled = true;
            this.clbTrain.Items.AddRange(new object[] {
            "1辆列车",
            "5辆列车",
            "10辆列车",
            "15辆列车",
            "20辆列车"});
            this.clbTrain.Location = new System.Drawing.Point(42, 24);
            this.clbTrain.Name = "clbTrain";
            this.clbTrain.Size = new System.Drawing.Size(383, 324);
            this.clbTrain.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSure);
            this.groupBox3.Location = new System.Drawing.Point(12, 407);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(808, 102);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // btnSure
            // 
            this.btnSure.Location = new System.Drawing.Point(284, 43);
            this.btnSure.Name = "btnSure";
            this.btnSure.Size = new System.Drawing.Size(121, 40);
            this.btnSure.TabIndex = 0;
            this.btnSure.Text = "确定";
            this.btnSure.UseVisualStyleBackColor = true;
            this.btnSure.Click += new System.EventHandler(this.btnSure_Click);
            // 
            // jlSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 521);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "jlSetting";
            this.Text = "jlSetting";
            this.Load += new System.EventHandler(this.jlSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox clbBianzu;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox clbTrain;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSure;
    }
}