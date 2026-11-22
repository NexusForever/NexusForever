using Microsoft.Extensions.Logging;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class WorldPropExtractor
    {
        #region Dependency Injection

        private readonly ILogger<WorldPropExtractor> log;
        private readonly Parameters parameters;
        private readonly AreaPropExtractor areaPropExtractor;

        public WorldPropExtractor(
            ILogger<WorldPropExtractor> log,
            Parameters parameters,
            AreaPropExtractor areaPropExtractor)
        {
            this.log               = log;
            this.parameters        = parameters;
            this.areaPropExtractor = areaPropExtractor;
        }

        #endregion

        public void Extract(string assetPath)
        {
            log.LogInformation($"Extracting props from world {assetPath}...");

            string directoryPath = Path.Combine(parameters.OutputPath, assetPath.Replace('\\', Path.DirectorySeparatorChar));
            if (!Directory.Exists(directoryPath))
            {
                log.LogWarning("World {assetPath} doesn't exist, skipping...", assetPath);
                return;
            }

            foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*.area"))
            {
                // skip any low quality grids
                if (filePath.Contains("_low", StringComparison.OrdinalIgnoreCase))
                    return;

                areaPropExtractor.Extract(filePath);
            }
        }
    }
}
