using Microsoft.Extensions.Logging;
using NexusForever.IO.Format;
using NexusForever.IO.Format.WildStar.I3;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    internal class I3PropExtractor : IPropExtractor
    {
        #region Dependency Injection

        private readonly ILogger<I3PropExtractor> log;
        private readonly IFileReader<I3File> i3FileReader;
        private readonly FileExtractor fileExtractor;

        public I3PropExtractor(
            ILogger<I3PropExtractor> log,
            IFileReader<I3File> i3FileReader,
            FileExtractor fileExtractor)
        {
            this.log           = log;
            this.i3FileReader  = i3FileReader;
            this.fileExtractor = fileExtractor;
        }

        #endregion

        public void Extract(string propPath)
        {
            if (!fileExtractor.Extract(propPath, out string filePath))
                return;

            IFile<I3File> i3File;
            try
            {
                i3File = i3FileReader.Read(filePath);
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to read I3 file {filePath}!", filePath);
                return;
            }

            string[] names = i3File.Model.NameData.Value.Split('\0');

            foreach (SectorInfo sector in i3File.Model.Sectors.Values)
            {
                string sectorPath = Path.Combine(Path.GetDirectoryName(propPath), $"{names[0]}.{names[sector.NameIndex]}.i3");
                fileExtractor.Extract(sectorPath, out string _);
            }
        }
    }
}
