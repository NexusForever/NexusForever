using System;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.MapGenerator.GameTable;
using NexusForever.Shared;
using NLog;

namespace NexusForever.MapGenerator
{
    internal static class MapGenerator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static ParserResult<Parameters> parserResult;

        #if DEBUG
        private const string Title = "NexusForever: Map Generator (DEBUG)";
        #else
        private const string Title = "NexusForever: Map Generator (RELEASE)";
        #endif

        private static int Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ArchiveManager>();
            services.AddSingleton<GameTableManager>();
            services.AddSingleton<ExtractionManager>();
            services.AddSingleton<GenerationManager>();

            LegacyServiceProvider.Provider = services.BuildServiceProvider();

            Console.Title = Title;

            parserResult = Parser.Default.ParseArguments<Parameters>(args);
            return parserResult.MapResult(parameters =>
            {
                try
                {
                    if (parameters.Prepare)
                        AssetPreparation.Run(parameters, ParameterOk);
                    else
                        ParameterOk(parameters);
                    log.Info("Finished!");
                    return 0;
                }
                catch (Exception exception)
                {
                    log.Error(exception, "Asset preparation failed.");
                    return 1;
                }
            }, errors => errors.All(e => e is HelpRequestedError or VersionRequestedError) ? 0 : 1);
        }

        private static void ParameterOk(Parameters parameters)
        {
            if (!Directory.Exists(parameters.PatchPath))
                throw new DirectoryNotFoundException($"Client Patch directory does not exist: {parameters.PatchPath}");

            if (!parameters.Extract && !parameters.Generate)
            {
                throw new ArgumentException("Specify --extract, --generate, or --prepare.");
            }

            if ((parameters.Extract || parameters.Generate) && !string.IsNullOrEmpty(parameters.OutputDir))
            {
                if (!Directory.Exists(parameters.OutputDir))
                    throw new DirectoryNotFoundException(parameters.OutputDir);
            }

            ArchiveManager.Instance.Initialise(parameters.PatchPath);
            GameTableManager.Instance.Initialise();

            if (parameters.Extract)
                ExtractionManager.Instance.Initialise(parameters.OutputDir);
            if (parameters.Generate)
            {
                GenerationManager.Instance.Initialise(parameters.OutputDir);

                var start = DateTime.UtcNow;
                if (parameters.WorldId.HasValue)
                    GenerationManager.Instance.GenerateWorld(parameters.WorldId.Value, parameters.GridX, parameters.GridY);
                else
                    GenerationManager.Instance.GenerateWorlds(true);

                TimeSpan span = DateTime.UtcNow - start;
                log.Info($"Generated base maps in {span.TotalSeconds}s.");
            }
        }
    }
}
