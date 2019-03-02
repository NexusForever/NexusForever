using Newtonsoft.Json;
using NexusForever.ServerLauncher.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NexusForever.ServerLauncher
{
    public partial class Form1 : Form
    {
        private const string configPath = "config.json";
        private Process authApplication, stsApplication, worldApplication;
        private bool IsAuthExited, IsStsExited, IsWorldExited = false;

        public Form1()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                SaveFile();
            }

            ConfigurationManager<ServerConfiguration>.Initialise(configPath);
            authPath.Text   = ConfigurationManager<ServerConfiguration>.Config.AuthPath;
            stsPath.Text    = ConfigurationManager<ServerConfiguration>.Config.StsPath;
            worldPath.Text  = ConfigurationManager<ServerConfiguration>.Config.WorldPath;
        }
        private void SaveFile()
        {
            ServerConfiguration configuration = new ServerConfiguration
            {
                AuthPath    = authPath.Text,
                StsPath     = stsPath.Text,
                WorldPath   = worldPath.Text
            };

            File.WriteAllText(configPath, JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }

        #region Browse Functions
        private void authBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "DLL Files(*.dll)|*.dll";
                dialog.Title = "Select the auth dll";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    authPath.Text = dialog.FileName;
                    SaveFile();
                }
            }
        }

        private void stsBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "DLL Files(*.dll)|*.dll";
                dialog.Title = "Select the sts dll";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    stsPath.Text = dialog.FileName;
                    SaveFile();
                }
            }
        }

        private void worldBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "DLL Files(*.dll)|*.dll";
                dialog.Title = "Select the world dll";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    worldPath.Text = dialog.FileName;
                    SaveFile();
                }
            }
        }
        #endregion

        #region Start/Stop Functions
        private void authStart_Click(object sender, EventArgs e)
        {
            authApplication             = new Process();
            authApplication.StartInfo   = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = authPath.Text
            };

            authApplication.Start();
            authApplication.EnableRaisingEvents = true;
            authApplication.Exited += new EventHandler(AuthExited);

            Log("Started Auth Server");
        }

        private void authStop_Click(object sender, EventArgs e)
        {
            if (!IsAuthExited)
            {
                authApplication.Kill();
                Log("Stopped Auth Server");
                IsAuthExited = false;
            }
        }

        private void startSts_Click(object sender, System.EventArgs e)
        {
            stsApplication              = new Process();
            stsApplication.StartInfo    = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = stsPath.Text
            };

            stsApplication.Start();
            stsApplication.EnableRaisingEvents = true;
            stsApplication.Exited += new EventHandler(StsExited);

            Log("Started Sts Server");
        }
        private void stsStop_Click(object sender, EventArgs e)
        {
            if (!IsStsExited)
            {
                stsApplication.Kill();
                Log("Stopped Sts Server");
                IsStsExited = false;
            }
        }

        private void worldStart_Click(object sender, EventArgs e)
        {
            worldApplication            = new Process();
            worldApplication.StartInfo  = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = worldPath.Text
            };

            worldApplication.Start();
            worldApplication.EnableRaisingEvents = true;
            worldApplication.Exited += new EventHandler(WorldExited);

            Log("Started World Server");
        }
        private void worldStop_Click(object sender, EventArgs e)
        {
            if (!IsWorldExited)
            {
                worldApplication.Kill();
                Log("Stopped World Server");
                IsWorldExited = false;
            }
        }

        private void AuthExited(object sender, EventArgs e) { IsAuthExited = true; }
        private void StsExited(object sender, EventArgs e) { IsStsExited = true; }
        private void WorldExited(object sender, EventArgs e) { IsWorldExited = true; }
        #endregion

        #region Log Box
        public void Log(string line)
        {
            string timeStamp            = DateTime.Now.ToLongTimeString();
            logWindow.SelectionStart    = logWindow.TextLength;
            logWindow.SelectionLength   = 0;
            logWindow.AppendText($"{timeStamp} | {line} \n");
            logWindow.ScrollToCaret();
        }
        #endregion
    }
}
