using Newtonsoft.Json;
using NexusForever.ClientConnector.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NexusForever.ClientConnector
{
    internal static class Program
    {
        public static string jsonFile = "config.json";

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

        static void Main()
        {
            if (!File.Exists(jsonFile))
            {
                Console.WriteLine("Type in your host name : ");
                string auth = Console.ReadLine();

                ClientConfiguration clientConfig = new ClientConfiguration
                {
                    HostName = auth
                };

                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(clientConfig));
                Main();
            }
            else
            {
                var file = File.ReadAllText(jsonFile);
                ConfigurationManager<ClientConfiguration>.Initialise(jsonFile);
                ClientConfiguration auth = JsonConvert.DeserializeObject<ClientConfiguration>(file);

                STARTUPINFO si = new STARTUPINFO();
                PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

                CreateProcess(
                    "WildStar64.exe",
                    $"/auth {auth.HostName} /authNc {auth.HostName} /lang en /patcher {auth.HostName} /SettingsKey WildStar /realmDataCenterId 9",
                IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);
            }
        }
    }
}
