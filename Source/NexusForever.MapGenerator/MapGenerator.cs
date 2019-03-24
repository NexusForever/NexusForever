using System;
using System.IO;
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

            if (args.Length == 0)
                throw new ArgumentException();

            if (!Directory.Exists(args[0]))
                throw new DirectoryNotFoundException();

            Flags flags = Flags.Extration | Flags.Generation;
            if (args.Length == 2)
                flags = (Flags)uint.Parse(args[1]);
                
            ArchiveManager.Initialise(args[0]);
            GameTableManager.Initialise();

            if ((flags & Flags.Extration) != 0)
                ExtractionManager.Initialise();
            if ((flags & Flags.Generation) != 0)
                GenerationManagaer.Initialise();

            log.Info("Finished!");
            Console.ReadLine();
        }
    }
}
