namespace AssemblyLinePreheatGUI_Csharp
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.progressBar1 = new ProgressBarEx.ProgressBarEx();
            this.progressBar2 = new ProgressBarEx.ProgressBarEx();
            this.progressBar3 = new ProgressBarEx.ProgressBarEx();
            this.progressBar4 = new ProgressBarEx.ProgressBarEx();
            this.progressBar5 = new ProgressBarEx.ProgressBarEx();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "端口 1：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(17, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "端口 2：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(17, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 14);
            this.label3.TabIndex = 7;
            this.label3.Text = "端口 3：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(17, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 8;
            this.label4.Text = "端口 4：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(17, 251);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 9;
            this.label5.Text = "端口 5：";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(17, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(337, 14);
            this.label6.TabIndex = 12;
            this.label6.Text = "进度条为绿色时，只需断开对应端口的设备即可。";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(354, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 14);
            this.label7.TabIndex = 14;
            this.label7.Text = "预热时间：";
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Transparent;
            this.progressBar1.Image = null;
            this.progressBar1.Location = new System.Drawing.Point(130, 51);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ShowPercentage = true;
            this.progressBar1.Size = new System.Drawing.Size(176, 23);
            this.progressBar1.Text = "progressBar1";
            // 
            // progressBar2
            // 
            this.progressBar2.BackColor = System.Drawing.Color.Transparent;
            this.progressBar2.Image = null;
            this.progressBar2.Location = new System.Drawing.Point(130, 101);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.ShowPercentage = true;
            this.progressBar2.Size = new System.Drawing.Size(176, 23);
            this.progressBar2.Text = "progressBar2";
            // 
            // progressBar3
            // 
            this.progressBar3.BackColor = System.Drawing.Color.Transparent;
            this.progressBar3.Image = null;
            this.progressBar3.Location = new System.Drawing.Point(130, 151);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.ShowPercentage = true;
            this.progressBar3.Size = new System.Drawing.Size(176, 23);
            this.progressBar3.Text = "progressBar3";
            // 
            // progressBar4
            // 
            this.progressBar4.BackColor = System.Drawing.Color.Transparent;
            this.progressBar4.Image = null;
            this.progressBar4.Location = new System.Drawing.Point(130, 201);
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.ShowPercentage = true;
            this.progressBar4.Size = new System.Drawing.Size(176, 23);
            this.progressBar4.Text = "progressBar4";
            // 
            // progressBar5
            // 
            this.progressBar5.BackColor = System.Drawing.Color.Transparent;
            this.progressBar5.Image = null;
            this.progressBar5.Location = new System.Drawing.Point(130, 251);
            this.progressBar5.Name = "progressBar5";
            this.progressBar5.ShowPercentage = true;
            this.progressBar5.Size = new System.Drawing.Size(176, 23);
            this.progressBar5.Text = "progressBar5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(363, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 14);
            this.label8.TabIndex = 22;
            this.label8.Text = "端口监控状态：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Lime;
            this.panel1.Location = new System.Drawing.Point(382, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(49, 18);
            this.panel1.TabIndex = 28;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 295);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.progressBar5);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "预热处理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private ProgressBarEx.ProgressBarEx progressBar1;
        private ProgressBarEx.ProgressBarEx progressBar2;
        private ProgressBarEx.ProgressBarEx progressBar3;
        private ProgressBarEx.ProgressBarEx progressBar4;
        private ProgressBarEx.ProgressBarEx progressBar5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
    }
}

