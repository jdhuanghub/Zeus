namespace DepthSensorTest
{
    partial class TestProgramWindow
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
            this.LoadXmlBtn = new System.Windows.Forms.Button();
            this.StartBtn = new System.Windows.Forms.Button();
            this.Frame = new System.Windows.Forms.Label();
            this.zeropixel = new System.Windows.Forms.Label();
            this.invalidpixel = new System.Windows.Forms.Label();
            this.validpixel = new System.Windows.Forms.Label();
            this.percentage1 = new System.Windows.Forms.Label();
            this.percentage2 = new System.Windows.Forms.Label();
            this.percentage3 = new System.Windows.Forms.Label();
            this.SetDistance = new System.Windows.Forms.Label();
            this.ErrorRange = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cBoxDevices = new System.Windows.Forms.ComboBox();
            this.centerDistance = new System.Windows.Forms.Label();
            this.cb_mirror = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // LoadXmlBtn
            // 
            this.LoadXmlBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadXmlBtn.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadXmlBtn.Location = new System.Drawing.Point(36, 272);
            this.LoadXmlBtn.Name = "LoadXmlBtn";
            this.LoadXmlBtn.Size = new System.Drawing.Size(133, 34);
            this.LoadXmlBtn.TabIndex = 0;
            this.LoadXmlBtn.Text = "Load customXML";
            this.LoadXmlBtn.UseVisualStyleBackColor = true;
            this.LoadXmlBtn.Click += new System.EventHandler(this.LoadXmlBtn_Click);
            // 
            // StartBtn
            // 
            this.StartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartBtn.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartBtn.Location = new System.Drawing.Point(389, 286);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(93, 34);
            this.StartBtn.TabIndex = 0;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // Frame
            // 
            this.Frame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Frame.AutoSize = true;
            this.Frame.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame.Location = new System.Drawing.Point(33, 73);
            this.Frame.Name = "Frame";
            this.Frame.Size = new System.Drawing.Size(70, 15);
            this.Frame.TabIndex = 1;
            this.Frame.Text = "NO.Frame:";
            // 
            // zeropixel
            // 
            this.zeropixel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zeropixel.AutoSize = true;
            this.zeropixel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zeropixel.Location = new System.Drawing.Point(56, 117);
            this.zeropixel.Name = "zeropixel";
            this.zeropixel.Size = new System.Drawing.Size(126, 15);
            this.zeropixel.TabIndex = 2;
            this.zeropixel.Text = "Pixel value of 0:";
            // 
            // invalidpixel
            // 
            this.invalidpixel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invalidpixel.AutoSize = true;
            this.invalidpixel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invalidpixel.Location = new System.Drawing.Point(56, 169);
            this.invalidpixel.Name = "invalidpixel";
            this.invalidpixel.Size = new System.Drawing.Size(189, 15);
            this.invalidpixel.TabIndex = 3;
            this.invalidpixel.Text = "Pixel value outside range:";
            // 
            // validpixel
            // 
            this.validpixel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.validpixel.AutoSize = true;
            this.validpixel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.validpixel.Location = new System.Drawing.Point(56, 221);
            this.validpixel.Name = "validpixel";
            this.validpixel.Size = new System.Drawing.Size(182, 15);
            this.validpixel.TabIndex = 4;
            this.validpixel.Text = "Pixel value inside range:";
            // 
            // percentage1
            // 
            this.percentage1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.percentage1.AutoSize = true;
            this.percentage1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentage1.Location = new System.Drawing.Point(300, 117);
            this.percentage1.Name = "percentage1";
            this.percentage1.Size = new System.Drawing.Size(105, 15);
            this.percentage1.TabIndex = 5;
            this.percentage1.Text = "percentage(%):";
            // 
            // percentage2
            // 
            this.percentage2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.percentage2.AutoSize = true;
            this.percentage2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentage2.Location = new System.Drawing.Point(300, 169);
            this.percentage2.Name = "percentage2";
            this.percentage2.Size = new System.Drawing.Size(105, 15);
            this.percentage2.TabIndex = 6;
            this.percentage2.Text = "percentage(%):";
            // 
            // percentage3
            // 
            this.percentage3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.percentage3.AutoSize = true;
            this.percentage3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentage3.Location = new System.Drawing.Point(300, 221);
            this.percentage3.Name = "percentage3";
            this.percentage3.Size = new System.Drawing.Size(105, 15);
            this.percentage3.TabIndex = 7;
            this.percentage3.Text = "percentage(%):";
            // 
            // SetDistance
            // 
            this.SetDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SetDistance.AutoSize = true;
            this.SetDistance.Location = new System.Drawing.Point(207, 272);
            this.SetDistance.Name = "SetDistance";
            this.SetDistance.Size = new System.Drawing.Size(101, 12);
            this.SetDistance.TabIndex = 8;
            this.SetDistance.Text = "Target Distance:";
            // 
            // ErrorRange
            // 
            this.ErrorRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorRange.AutoSize = true;
            this.ErrorRange.Location = new System.Drawing.Point(207, 294);
            this.ErrorRange.Name = "ErrorRange";
            this.ErrorRange.Size = new System.Drawing.Size(77, 12);
            this.ErrorRange.TabIndex = 9;
            this.ErrorRange.Text = "Error Range:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Device:";
            // 
            // cBoxDevices
            // 
            this.cBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxDevices.FormattingEnabled = true;
            this.cBoxDevices.Location = new System.Drawing.Point(78, 25);
            this.cBoxDevices.Name = "cBoxDevices";
            this.cBoxDevices.Size = new System.Drawing.Size(178, 20);
            this.cBoxDevices.TabIndex = 11;
            this.cBoxDevices.SelectedIndexChanged += new System.EventHandler(this.cBoxDevices_SelectedIndexChanged);
            // 
            // centerDistance
            // 
            this.centerDistance.AutoSize = true;
            this.centerDistance.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.centerDistance.Location = new System.Drawing.Point(171, 74);
            this.centerDistance.Name = "centerDistance";
            this.centerDistance.Size = new System.Drawing.Size(126, 14);
            this.centerDistance.TabIndex = 12;
            this.centerDistance.Text = "Center Distance: ";
            // 
            // cb_mirror
            // 
            this.cb_mirror.AutoSize = true;
            this.cb_mirror.Location = new System.Drawing.Point(378, 74);
            this.cb_mirror.Name = "cb_mirror";
            this.cb_mirror.Size = new System.Drawing.Size(60, 16);
            this.cb_mirror.TabIndex = 13;
            this.cb_mirror.Text = "Mirror";
            this.cb_mirror.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Blue;
            this.panel1.Location = new System.Drawing.Point(27, 117);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(18, 15);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Red;
            this.panel2.Location = new System.Drawing.Point(27, 169);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(18, 15);
            this.panel2.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Lime;
            this.panel3.Location = new System.Drawing.Point(27, 221);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(18, 15);
            this.panel3.TabIndex = 16;
            // 
            // TestProgramWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 332);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cb_mirror);
            this.Controls.Add(this.centerDistance);
            this.Controls.Add(this.cBoxDevices);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorRange);
            this.Controls.Add(this.SetDistance);
            this.Controls.Add(this.percentage3);
            this.Controls.Add(this.percentage2);
            this.Controls.Add(this.percentage1);
            this.Controls.Add(this.validpixel);
            this.Controls.Add(this.invalidpixel);
            this.Controls.Add(this.zeropixel);
            this.Controls.Add(this.Frame);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.LoadXmlBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestProgramWindow";
            this.Text = "TestProgram";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestProgramWindow_FormClosing);
            this.Load += new System.EventHandler(this.TestProgramWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadXmlBtn;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Label Frame;
        private System.Windows.Forms.Label zeropixel;
        private System.Windows.Forms.Label invalidpixel;
        private System.Windows.Forms.Label validpixel;
        private System.Windows.Forms.Label percentage1;
        private System.Windows.Forms.Label percentage2;
        private System.Windows.Forms.Label percentage3;
        private System.Windows.Forms.Label SetDistance;
        private System.Windows.Forms.Label ErrorRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cBoxDevices;
        private System.Windows.Forms.Label centerDistance;
        private System.Windows.Forms.CheckBox cb_mirror;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}

