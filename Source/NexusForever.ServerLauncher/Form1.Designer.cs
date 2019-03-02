namespace NexusForever.ServerLauncher
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
            this.logWindow = new System.Windows.Forms.TextBox();
            this.authPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stsPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.worldPath = new System.Windows.Forms.TextBox();
            this.authBrowse = new System.Windows.Forms.Button();
            this.stsBrowse = new System.Windows.Forms.Button();
            this.worldBrowse = new System.Windows.Forms.Button();
            this.worldStart = new System.Windows.Forms.Button();
            this.authStart = new System.Windows.Forms.Button();
            this.startSts = new System.Windows.Forms.Button();
            this.stsStop = new System.Windows.Forms.Button();
            this.authStop = new System.Windows.Forms.Button();
            this.worldStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logWindow
            // 
            this.logWindow.Location = new System.Drawing.Point(12, 229);
            this.logWindow.Multiline = true;
            this.logWindow.Name = "logWindow";
            this.logWindow.ReadOnly = true;
            this.logWindow.Size = new System.Drawing.Size(340, 124);
            this.logWindow.TabIndex = 0;
            // 
            // authPath
            // 
            this.authPath.Location = new System.Drawing.Point(12, 25);
            this.authPath.Name = "authPath";
            this.authPath.Size = new System.Drawing.Size(244, 20);
            this.authPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Auth Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Sts Server";
            // 
            // stsPath
            // 
            this.stsPath.Location = new System.Drawing.Point(12, 100);
            this.stsPath.Name = "stsPath";
            this.stsPath.Size = new System.Drawing.Size(244, 20);
            this.stsPath.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "World Server";
            // 
            // worldPath
            // 
            this.worldPath.Location = new System.Drawing.Point(12, 174);
            this.worldPath.Name = "worldPath";
            this.worldPath.Size = new System.Drawing.Size(244, 20);
            this.worldPath.TabIndex = 5;
            // 
            // authBrowse
            // 
            this.authBrowse.Location = new System.Drawing.Point(262, 24);
            this.authBrowse.Name = "authBrowse";
            this.authBrowse.Size = new System.Drawing.Size(90, 21);
            this.authBrowse.TabIndex = 7;
            this.authBrowse.Text = "Browse";
            this.authBrowse.UseVisualStyleBackColor = true;
            this.authBrowse.Click += new System.EventHandler(this.authBrowse_Click);
            // 
            // stsBrowse
            // 
            this.stsBrowse.Location = new System.Drawing.Point(262, 99);
            this.stsBrowse.Name = "stsBrowse";
            this.stsBrowse.Size = new System.Drawing.Size(90, 22);
            this.stsBrowse.TabIndex = 8;
            this.stsBrowse.Text = "Browse";
            this.stsBrowse.UseVisualStyleBackColor = true;
            this.stsBrowse.Click += new System.EventHandler(this.stsBrowse_Click);
            // 
            // worldBrowse
            // 
            this.worldBrowse.Location = new System.Drawing.Point(262, 173);
            this.worldBrowse.Name = "worldBrowse";
            this.worldBrowse.Size = new System.Drawing.Size(90, 22);
            this.worldBrowse.TabIndex = 9;
            this.worldBrowse.Text = "Browse";
            this.worldBrowse.UseVisualStyleBackColor = true;
            this.worldBrowse.Click += new System.EventHandler(this.worldBrowse_Click);
            // 
            // worldStart
            // 
            this.worldStart.Location = new System.Drawing.Point(12, 200);
            this.worldStart.Name = "worldStart";
            this.worldStart.Size = new System.Drawing.Size(169, 23);
            this.worldStart.TabIndex = 11;
            this.worldStart.Text = "Start World";
            this.worldStart.UseVisualStyleBackColor = true;
            this.worldStart.Click += new System.EventHandler(this.worldStart_Click);
            // 
            // authStart
            // 
            this.authStart.Location = new System.Drawing.Point(12, 51);
            this.authStart.Name = "authStart";
            this.authStart.Size = new System.Drawing.Size(169, 23);
            this.authStart.TabIndex = 12;
            this.authStart.Text = "Start Auth";
            this.authStart.UseVisualStyleBackColor = true;
            this.authStart.Click += new System.EventHandler(this.authStart_Click);
            // 
            // startSts
            // 
            this.startSts.Location = new System.Drawing.Point(12, 126);
            this.startSts.Name = "startSts";
            this.startSts.Size = new System.Drawing.Size(169, 23);
            this.startSts.TabIndex = 13;
            this.startSts.Text = "Start Sts";
            this.startSts.UseVisualStyleBackColor = true;
            this.startSts.Click += new System.EventHandler(this.startSts_Click);
            // 
            // stsStop
            // 
            this.stsStop.Location = new System.Drawing.Point(187, 127);
            this.stsStop.Name = "stsStop";
            this.stsStop.Size = new System.Drawing.Size(165, 23);
            this.stsStop.TabIndex = 16;
            this.stsStop.Text = "Stop Sts";
            this.stsStop.UseVisualStyleBackColor = true;
            this.stsStop.Click += new System.EventHandler(this.stsStop_Click);
            // 
            // authStop
            // 
            this.authStop.Location = new System.Drawing.Point(187, 51);
            this.authStop.Name = "authStop";
            this.authStop.Size = new System.Drawing.Size(165, 23);
            this.authStop.TabIndex = 15;
            this.authStop.Text = "Stop Auth";
            this.authStop.UseVisualStyleBackColor = true;
            this.authStop.Click += new System.EventHandler(this.authStop_Click);
            // 
            // worldStop
            // 
            this.worldStop.Location = new System.Drawing.Point(187, 200);
            this.worldStop.Name = "worldStop";
            this.worldStop.Size = new System.Drawing.Size(165, 23);
            this.worldStop.TabIndex = 14;
            this.worldStop.Text = "Stop World";
            this.worldStop.UseVisualStyleBackColor = true;
            this.worldStop.Click += new System.EventHandler(this.worldStop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 365);
            this.Controls.Add(this.stsStop);
            this.Controls.Add(this.authStop);
            this.Controls.Add(this.worldStop);
            this.Controls.Add(this.startSts);
            this.Controls.Add(this.authStart);
            this.Controls.Add(this.worldStart);
            this.Controls.Add(this.worldBrowse);
            this.Controls.Add(this.stsBrowse);
            this.Controls.Add(this.authBrowse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.worldPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.stsPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.authPath);
            this.Controls.Add(this.logWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "NexusForever Server Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logWindow;
        private System.Windows.Forms.TextBox authPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox stsPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox worldPath;
        private System.Windows.Forms.Button authBrowse;
        private System.Windows.Forms.Button stsBrowse;
        private System.Windows.Forms.Button worldBrowse;
        private System.Windows.Forms.Button worldStart;
        private System.Windows.Forms.Button authStart;
        private System.Windows.Forms.Button startSts;
        private System.Windows.Forms.Button stsStop;
        private System.Windows.Forms.Button authStop;
        private System.Windows.Forms.Button worldStop;
    }
}

