using System;
using System.IO;
using CommandLine;
using CommandLine.Text;
using NexusForever.MapGenerator.GameTable;
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

        private static void Main(string[] args)
        {
            Console.Title = Title;

            parserResult = Parser.Default.ParseArguments<Parameters>(args);
            parserResult.WithParsed(ParameterOk);

            log.Info("Finished!");
            Console.ReadLine();
        }

        private static void ParameterOk(Parameters parameters)
        {
            if (!Directory.Exists(parameters.PatchPath))
                throw new DirectoryNotFoundException();

            if (!parameters.Extract && !parameters.Generate)
            {
                log.Warn("Please specify the Extract or Generate parameter");
                log.Info(GetHelp());
                return;
            }

            ArchiveManager.Instance.Initialise(parameters.PatchPath);
            GameTableManager.Instance.Initialise();

            if (parameters.Extract)
                ExtractionManager.Instance.Initialise();
            if (parameters.Generate)
            {
                GenerationManager.Instance.Initialise();

                var start = DateTime.UtcNow;
                if (parameters.WorldId.HasValue)
                    GenerationManager.Instance.GenerateWorld(parameters.WorldId.Value, parameters.GridX, parameters.GridY);
                else
                    GenerationManager.Instance.GenerateWorlds(true);

                TimeSpan span = DateTime.UtcNow - start;
                log.Info($"Generated base maps in {span.TotalSeconds}s.");
            }
        }

        private static string GetHelp()
        {
            return HelpText.AutoBuild(parserResult, h => h, e => e);
        }
    }
}
