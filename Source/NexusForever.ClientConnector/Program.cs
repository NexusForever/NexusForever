using Newtonsoft.Json;
using NexusForever.ClientConnector.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;

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

                ClientConfiguration clientConfig = new ClientConfiguration
                {
                    HostName = hostName
                };

                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(clientConfig));

                LaunchClient(hostName);
            }
            else
            {
                ConfigurationManager<ClientConfiguration>.Initialise(jsonFile);
                LaunchClient(ConfigurationManager<ClientConfiguration>.Config.HostName);
            }
        }

        private static void LaunchClient(string hostName)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            CreateProcess(
                "WildStar64.exe",
                $"/auth {hostName} /authNc {hostName} /lang en /patcher {hostName} /SettingsKey WildStar /realmDataCenterId 9",
                IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);
        }
    }
}
