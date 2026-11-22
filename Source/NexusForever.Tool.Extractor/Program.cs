using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.GameTable;
using NexusForever.GameTable.Configuration.Model;
using NexusForever.IO.Format.WildStar;
using NexusForever.Tool.Extractor.Extract;
using NLog.Extensions.Logging;

namespace NexusForever.Tool.Extractor
{
    internal static class Program
    {
#if DEBUG
        private const string Title = "NexusForever: Extractor (DEBUG)";
#else
        private const string Title = "NexusForever: Extractor (RELEASE)";
#endif

        private async static Task Main(string[] args)
        {
            Console.Title = Title;

            await Parser.Default.ParseArguments<Parameters>(args)
                .WithParsedAsync(async parameters =>
                {
                    string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile("Logging.json", true)
                        .Build();

                    IServiceCollection sc = new ServiceCollection();
                    sc.AddTransient(p => parameters);

                    sc.AddLogging(l =>
                    {
                        l.ClearProviders()
                            .AddConfiguration(configuration.GetSection("Logging"))
                            .AddNLog(new NLogProviderOptions
                            {
                                RemoveLoggerFactoryFilter = false
                            });
                    });

                    sc.AddIOFormatWildStar();
                    sc.AddGameTable(c =>
                    {
                        c.GameTablePath = Path.Combine(parameters.OutputPath, "DB");
                        c.Cache         = new CacheConfig
                        {
                            UseCache = false
                        };
                    });

                    sc.AddSingleton<Runner>();
                    sc.AddSingleton<ArchiveManager>();

                    sc.AddExtract();

                    Runner runner = sc.BuildServiceProvider().GetService<Runner>();
                    await runner.Run();
                });
        }
    }
}