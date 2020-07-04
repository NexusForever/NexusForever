using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NexusForever.ClientConnector.Configuration;
using NexusForever.ClientConnector.Native;

namespace NexusForever.ClientConnector
{
    internal static class Program
    {
        private const string jsonFile = "config.json";

        [DllImport("kernel32.dll")]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
        );

        public static void Main()
        {
            if (!File.Exists(jsonFile))
            {
                Console.Write("Type in your host name: ");
                string hostName = Console.ReadLine();

                Console.Write("Type in your language: [en,de]");
                string language = Console.ReadLine();

                ClientConfiguration clientConfig = new ClientConfiguration
                {
                    HostName = hostName,
                    Language = language
                };

                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(clientConfig));
            }

            ConfigurationManager<ClientConfiguration>.Initialise(jsonFile);
            LaunchClient(ConfigurationManager<ClientConfiguration>.Config);
        }

        private static void LaunchClient(ClientConfiguration config)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            string client = "WildStar64.exe";
            if (!File.Exists(client))
                client = "WildStar32.exe";

            CreateProcess(client,
                $"/auth {config.HostName} /authNc {config.HostName} /lang {config.Language} /patcher {config.HostName} /SettingsKey WildStar /realmDataCenterId 9",
                IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);
        }
    }
}
