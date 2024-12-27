using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using NexusForever.Database;
using NexusForever.Network.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.StsServer.Network;
using NLog;
using NLog.Extensions.Logging;

namespace NexusForever.StsServer
{
    internal static class StsServer
    {
        #if DEBUG
        private const string Title = "NexusForever: STS Server (DEBUG)";
        #else
        private const string Title = "NexusForever: STS Server (RELEASE)";
        #endif

        private static readonly NLog.ILogger log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            IHostBuilder builder = new HostBuilder()
                .ConfigureLogging(lb =>
                {
                    lb.AddNLog();
                })
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("StsServer.json", false);
                })
                .ConfigureServices((hb, sc) =>
                {
                    sc.AddHostedService<HostedService>();

                    sc.AddOptions<NetworkConfig>()
                        .Bind(hb.Configuration.GetSection("Network"));

                    sc.AddSingletonLegacy<ISharedConfiguration, SharedConfiguration>();

                    sc.AddDatabase();
                    sc.AddStsNetwork();
                    sc.AddShared();
                })
                .UseWindowsService()
                .UseSystemd();

            if (!WindowsServiceHelpers.IsWindowsService() && !SystemdHelpers.IsSystemdService())
                Console.Title = Title;

            try
            {
                IHost host = builder.Build();
                host.Run();
            }
            catch (Exception e)
            {
                log.Fatal(e);
            }
        }
    }
}
