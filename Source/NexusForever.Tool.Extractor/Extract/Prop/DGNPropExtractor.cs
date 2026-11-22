using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.IO.Format;
using NexusForever.IO.Format.Wildstar.DGN;
using NexusForever.IO.Format.Wildstar.DGN.Dressing;
using NexusForever.IO.Format.WildStar.Area.Static;

namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class DGNPropExtractor : IPropExtractor
    {
        #region Dependency Injection

        private readonly ILogger<DGNPropExtractor> log;
        private readonly IFileReader<DNGFile> dngFileReader;
        private readonly IFileReader<DGNDressingFile> dngDressingFileReader;
        private readonly FileExtractor fileExtractor;
        private readonly IPropExtractor i3PropExtractor;

        public DGNPropExtractor(
            ILogger<DGNPropExtractor> log,
            IFileReader<DNGFile> dngFileReader,
            IFileReader<DGNDressingFile> dngDressingFileReader,
            FileExtractor fileExtractor,
            [FromKeyedServices(PropType.I3)]
            IPropExtractor i3PropExtractor)
        {
            this.log                   = log;
            this.dngFileReader         = dngFileReader;
            this.dngDressingFileReader = dngDressingFileReader;
            this.fileExtractor         = fileExtractor;
            this.i3PropExtractor       = i3PropExtractor;
        }

        #endregion

        public void Extract(string propPath)
        {
            if (!fileExtractor.Extract(propPath, out string filePath))
                return;

            IFile<DNGFile> dngFile;
            try
            {
                dngFile = dngFileReader.Read(filePath);
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to read DNG file {filePath}!", filePath);
                return;
            }

            if (dngFile.Model.I3File.Value != null)
                i3PropExtractor.Extract(dngFile.Model.I3File.Value);

            foreach (DressingInfo dressing in dngFile.Model.Dressings.Values)
            {
                if (!fileExtractor.Extract(dressing.File.Value, out string dressingFilePath))
                    continue;

                IFile<DGNDressingFile> dngDressingFile;
                try
                {
                    dngDressingFile = dngDressingFileReader.Read(dressingFilePath);
                }
                catch (Exception ex)
                {
                    log.LogWarning(ex, "Failed to read DNG dressing file {filePath}!", filePath);
                    return;
                }

                foreach (Model model in dngDressingFile.Model.Models.Values)
                    fileExtractor.Extract(model.Path.Value, out string _);
            }
        }
    }
}
