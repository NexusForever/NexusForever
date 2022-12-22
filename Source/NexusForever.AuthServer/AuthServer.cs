using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace NexusForever.AuthServer
{
    internal static class AuthServer
    {
        #if DEBUG
        private const string Title = "NexusForever: Authentication Server (DEBUG)";
        #else
        private const string Title = "NexusForever: Authentication Server (RELEASE)";
        #endif

        private static readonly NLog.ILogger log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            SharedConfiguration.Instance.Initialise<AuthServerConfiguration>("AuthServer.json");

            IHostBuilder builder = new HostBuilder()
                .ConfigureLogging(lb =>
                {
                    // only applicable to logging done through host
                    // other logging is still done directly though NLog
                    lb.ClearProviders()
                        .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace)
                        .AddNLog();
                })
                .ConfigureServices(sc =>
                {
                    sc.AddHostedService<HostedService>();
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
