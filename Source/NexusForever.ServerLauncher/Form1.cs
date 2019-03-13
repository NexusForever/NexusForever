using Newtonsoft.Json;
using NexusForever.ServerLauncher.Configuration;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NexusForever.ServerLauncher
{
    public partial class Form1 : Form
    {
        private const string configPath = "config.json";
        private Process authApplication, stsApplication, worldApplication;
        private bool IsAuthExited, IsStsExited, IsWorldExited = false;
        private string AuthPath, StsPath, WorldPath;

        public Form1()
        {
            InitializeComponent();
            LoadConfig();
        }

        #region File Loading/Saving
        private void LoadConfig()
        {
            if (!File.Exists(configPath))
                SaveFile();

            ConfigurationManager<ServerConfiguration>.Initialise(configPath);
            if (!Directory.Exists(ConfigurationManager<ServerConfiguration>.Config.MainPath))
                Log($"Dir does not exist: '{ConfigurationManager<ServerConfiguration>.Config.MainPath}'", true);
            else
            {
                sourcePath.Text = ConfigurationManager<ServerConfiguration>.Config.MainPath;
                LoadFiles();
                SaveFile();
            }
        }
        private void SaveFile()
        {
            ServerConfiguration configuration = new ServerConfiguration
            {
                MainPath    = sourcePath.Text,
                AuthPath    = AuthPath,
                StsPath     = StsPath,
                WorldPath   = WorldPath
            };

            File.WriteAllText(configPath, JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }
        #endregion

        #region Browse Functions
        private void browseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sourcePath.Text = dialog.SelectedPath;
                    Log($"Selected: {dialog.SelectedPath}");

                    LoadFiles();
                }
            }
        }
        private void LoadFiles()
        {
            string[] files = Directory.GetFiles(sourcePath.Text, "*.dll", SearchOption.AllDirectories);
            foreach (string filePath in files)
            {
                if (filePath.Contains("obj"))
                    continue;
                else
                {
                    string filename = Path.GetFileName(filePath);

                    switch (filename)
                    {
                        case "NexusForever.StsServer.dll":
                            StsPath = filePath;
                            break;
                        case "NexusForever.AuthServer.dll":
                            AuthPath = filePath;
                            break;
                        case "NexusForever.WorldServer.dll":
                            WorldPath = filePath;
                            break;
                    }
                }
            }

            SaveFile();
        }
        #endregion

        #region Start/Stop Functions
        private void authStart_Click(object sender, EventArgs e)
        {
            authApplication             = new Process();
            authApplication.StartInfo   = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = AuthPath
            };

            authApplication.Start();
            authApplication.EnableRaisingEvents = true;
            authApplication.Exited += new EventHandler(AuthExited);
            IsAuthExited = false;

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

        private void startSts_Click(object sender, EventArgs e)
        {
            stsApplication              = new Process();
            stsApplication.StartInfo    = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = StsPath
            };

            stsApplication.Start();
            stsApplication.EnableRaisingEvents = true;
            stsApplication.Exited += new EventHandler(StsExited);
            IsStsExited = false;

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
                Arguments = WorldPath
            };

            worldApplication.Start();
            worldApplication.EnableRaisingEvents = true;
            worldApplication.Exited += new EventHandler(WorldExited);
            IsWorldExited = false;

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
        public void Log(string line, bool error = false)
        {
            string timeStamp            = DateTime.Now.ToLongTimeString();
            Color color;
            if (error)
                color = Color.Red;
            else
                color = Color.Black;

            logWindow.SelectionStart    = logWindow.TextLength;
            logWindow.SelectionLength   = 0;
            logWindow.SelectionColor    = color;
            logWindow.AppendText($"{timeStamp} | {line} \n");
            logWindow.ScrollToCaret();
        }
        #endregion
    }
}
