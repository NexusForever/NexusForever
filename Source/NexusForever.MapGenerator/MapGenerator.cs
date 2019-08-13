using System;
using System.IO;
using CommandLine;
using NexusForever.MapGenerator.GameTable;
using NLog;

namespace NexusForever.MapGenerator
{
    internal static class MapGenerator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #if DEBUG
        private const string Title = "NexusForever: Map Generator (DEBUG)";
        #else
        private const string Title = "NexusForever: Map Generator (RELEASE)";
        #endif

        private static void Main(string[] args)
        {
            Console.Title = Title;

            Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(ParameterOk);

            log.Info("Finished!");
            Console.ReadLine();
        }

        private static void ParameterOk(Parameters parameters)
        {
            if (!Directory.Exists(parameters.PatchPath))
                throw new DirectoryNotFoundException();

            ArchiveManager.Initialise(parameters.PatchPath);
            GameTableManager.Initialise();

            if (parameters.Extract)
                ExtractionManager.Initialise();
            if (parameters.Generate)
            {
                GenerationManager.Initialise();

                var start = DateTime.UtcNow;
                if (parameters.WorldId.HasValue)
                    GenerationManager.GenerateWorld(parameters.WorldId.Value, parameters.GridX, parameters.GridY);
                else
                    GenerationManager.GenerateWorlds(true);

                TimeSpan span = DateTime.UtcNow - start;
                log.Info($"Generated base maps in {span.TotalSeconds}s.");
            }
        }
    }
}
