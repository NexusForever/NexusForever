using Microsoft.Extensions.Logging;
using NexusForever.IO.Format;
using NexusForever.IO.Format.WildStar.Area;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class AreaPropExtractor
    {
        #region Dependency Injection

        private readonly ILogger<AreaPropExtractor> log;
        private readonly IFileReader<AreaFile> areaFileLoader;
        private readonly PropExtractorFactory propExtractorFactory;

        public AreaPropExtractor(
            ILogger<AreaPropExtractor> log,
            IFileReader<AreaFile> areaFileLoader,
            PropExtractorFactory propExtractorFactory)
        {
            this.log                  = log;
            this.areaFileLoader       = areaFileLoader;
            this.propExtractorFactory = propExtractorFactory;
        }

        #endregion

        public void Extract(string filePath)
        {
            log.LogInformation("Extracting props from {filePath}...", filePath);

            IFile<AreaFile> areaFile;
            try
            {
                areaFile = areaFileLoader.Read(filePath);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to read area file {filePath}!", filePath);
                return;
            }

            PropChunk propChunk = areaFile.Model.Chunks.OfType<PropChunk>().SingleOrDefault();
            if (propChunk == null)
                return;

            foreach (PropChunk.Prop prop in propChunk.Props)
            {
                IPropExtractor propExtractor = propExtractorFactory.Resolve(prop.PropType);
                propExtractor?.Extract(prop.Path);
            }
        }
    }
}
