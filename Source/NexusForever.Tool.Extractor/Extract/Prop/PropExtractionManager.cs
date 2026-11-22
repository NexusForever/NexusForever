using Microsoft.Extensions.Logging;
using NexusForever.GameTable;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class PropExtractionManager
    {
        #region Dependency Injection

        private readonly ILogger<PropExtractionManager> log;
        private readonly IGameTableManager gameTableManager;
        private readonly WorldPropExtractor worldPropExtractor;

        public PropExtractionManager(
            ILogger<PropExtractionManager> log,
            IGameTableManager gameTableManager,
            WorldPropExtractor worldPropExtractor)
        {
            this.log                = log;
            this.gameTableManager   = gameTableManager;
            this.worldPropExtractor = worldPropExtractor;
        }

        #endregion

        public void Extract()
        {
            log.LogInformation("Extracting props...");

            foreach (string assetPath in gameTableManager.World.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.AssetPath))
                .Select(e => e.AssetPath)
                .Distinct())
                worldPropExtractor.Extract(assetPath);
        }
    }
}
