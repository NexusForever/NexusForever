using System;
using System.Runtime.InteropServices;

namespace NexusForever.ClientConnector
{
    class Program
    {
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

        static void Main(string[] args)
        {
            Console.WriteLine("Type in your host name : ");
            string auth = Console.ReadLine();

            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            CreateProcess(
                "WildStar64.exe",
                $"/auth {auth} /authNc {auth} /lang en /patcher {auth} /SettingsKey WildStar /realmDataCenterId 9",
            IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);
        }
    }
}
