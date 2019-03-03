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
            this.worldStart = new System.Windows.Forms.Button();
            this.authStart = new System.Windows.Forms.Button();
            this.startSts = new System.Windows.Forms.Button();
            this.stsStop = new System.Windows.Forms.Button();
            this.authStop = new System.Windows.Forms.Button();
            this.worldStop = new System.Windows.Forms.Button();
            this.browseSource = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.sourcePath = new System.Windows.Forms.TextBox();
            this.logWindow = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // worldStart
            // 
            this.worldStart.Location = new System.Drawing.Point(12, 118);
            this.worldStart.Name = "worldStart";
            this.worldStart.Size = new System.Drawing.Size(169, 23);
            this.worldStart.TabIndex = 11;
            this.worldStart.Text = "Start World Server";
            this.worldStart.UseVisualStyleBackColor = true;
            this.worldStart.Click += new System.EventHandler(this.worldStart_Click);
            // 
            // authStart
            // 
            this.authStart.Location = new System.Drawing.Point(12, 60);
            this.authStart.Name = "authStart";
            this.authStart.Size = new System.Drawing.Size(169, 23);
            this.authStart.TabIndex = 12;
            this.authStart.Text = "Start Auth Server";
            this.authStart.UseVisualStyleBackColor = true;
            this.authStart.Click += new System.EventHandler(this.authStart_Click);
            // 
            // startSts
            // 
            this.startSts.Location = new System.Drawing.Point(12, 89);
            this.startSts.Name = "startSts";
            this.startSts.Size = new System.Drawing.Size(169, 23);
            this.startSts.TabIndex = 13;
            this.startSts.Text = "Start Sts Server";
            this.startSts.UseVisualStyleBackColor = true;
            this.startSts.Click += new System.EventHandler(this.startSts_Click);
            // 
            // stsStop
            // 
            this.stsStop.Location = new System.Drawing.Point(187, 89);
            this.stsStop.Name = "stsStop";
            this.stsStop.Size = new System.Drawing.Size(165, 23);
            this.stsStop.TabIndex = 16;
            this.stsStop.Text = "Stop Sts Server";
            this.stsStop.UseVisualStyleBackColor = true;
            this.stsStop.Click += new System.EventHandler(this.stsStop_Click);
            // 
            // authStop
            // 
            this.authStop.Location = new System.Drawing.Point(187, 60);
            this.authStop.Name = "authStop";
            this.authStop.Size = new System.Drawing.Size(165, 23);
            this.authStop.TabIndex = 15;
            this.authStop.Text = "Stop Auth Server";
            this.authStop.UseVisualStyleBackColor = true;
            this.authStop.Click += new System.EventHandler(this.authStop_Click);
            // 
            // worldStop
            // 
            this.worldStop.Location = new System.Drawing.Point(187, 118);
            this.worldStop.Name = "worldStop";
            this.worldStop.Size = new System.Drawing.Size(165, 23);
            this.worldStop.TabIndex = 14;
            this.worldStop.Text = "Stop World Server";
            this.worldStop.UseVisualStyleBackColor = true;
            this.worldStop.Click += new System.EventHandler(this.worldStop_Click);
            // 
            // browseSource
            // 
            this.browseSource.Location = new System.Drawing.Point(262, 28);
            this.browseSource.Name = "browseSource";
            this.browseSource.Size = new System.Drawing.Size(90, 21);
            this.browseSource.TabIndex = 20;
            this.browseSource.Text = "Browse";
            this.browseSource.UseVisualStyleBackColor = true;
            this.browseSource.Click += new System.EventHandler(this.browseSource_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Source Folder";
            // 
            // sourcePath
            // 
            this.sourcePath.Location = new System.Drawing.Point(12, 29);
            this.sourcePath.Name = "sourcePath";
            this.sourcePath.ReadOnly = true;
            this.sourcePath.Size = new System.Drawing.Size(244, 20);
            this.sourcePath.TabIndex = 17;
            // 
            // logWindow
            // 
            this.logWindow.Location = new System.Drawing.Point(12, 156);
            this.logWindow.Name = "logWindow";
            this.logWindow.ReadOnly = true;
            this.logWindow.Size = new System.Drawing.Size(340, 153);
            this.logWindow.TabIndex = 21;
            this.logWindow.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 322);
            this.Controls.Add(this.logWindow);
            this.Controls.Add(this.browseSource);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sourcePath);
            this.Controls.Add(this.stsStop);
            this.Controls.Add(this.authStop);
            this.Controls.Add(this.worldStop);
            this.Controls.Add(this.startSts);
            this.Controls.Add(this.authStart);
            this.Controls.Add(this.worldStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "NexusForever Server Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button worldStart;
        private System.Windows.Forms.Button authStart;
        private System.Windows.Forms.Button startSts;
        private System.Windows.Forms.Button stsStop;
        private System.Windows.Forms.Button authStop;
        private System.Windows.Forms.Button worldStop;
        private System.Windows.Forms.Button browseSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox sourcePath;
        private System.Windows.Forms.RichTextBox logWindow;
    }
}

