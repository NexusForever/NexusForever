using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Tool.Extractor.Extract;
using NexusForever.Tool.Extractor.Extract.Prop;

namespace NexusForever.Tool.Extractor
{
    public class Runner
    {
        #region Dependency Injection

        private readonly ILogger<Runner> log;
        private readonly Parameters parameters;
        private readonly ArchiveManager archiveManager;
        private readonly GameTableExtractionManager gameTableExtractor;
        private readonly AreaExtractionManager areaExtractionManager;
        private readonly PropExtractionManager propExtractionManager;
        private readonly IGameTableManager gameTableManager;

        public Runner(
            ILogger<Runner> log,
            Parameters parameters,
            ArchiveManager archiveManager,
            GameTableExtractionManager gameTableExtractor,
            AreaExtractionManager areaExtractionManager,
            PropExtractionManager propExtractionManager,
            IGameTableManager gameTableManager)
        {
            this.log                   = log;
            this.parameters            = parameters;
            this.archiveManager        = archiveManager;
            this.gameTableExtractor    = gameTableExtractor;
            this.areaExtractionManager = areaExtractionManager;
            this.propExtractionManager = propExtractionManager;
            this.gameTableManager      = gameTableManager;
        }

        #endregion

        public async Task Run()
        {
            var start = Stopwatch.GetTimestamp();

            archiveManager.Initialise();

            if (parameters.ExtractGameTable)
                gameTableExtractor.Extract();

            if (parameters.ExtractArea || parameters.ExtractProp)
            {
                var loader = new GameTableLoader();
                loader.AddGameTable<WorldEntry>();
                await gameTableManager.Initialise(loader);
            }

            if (parameters.ExtractArea)
                areaExtractionManager.Extract();

            if (parameters.ExtractProp)
                propExtractionManager.Extract();

            var end = Stopwatch.GetTimestamp();
            log.LogInformation("Finished in {Elapsed} ms.", Stopwatch.GetElapsedTime(start, end).TotalSeconds);
        }
    }
}
