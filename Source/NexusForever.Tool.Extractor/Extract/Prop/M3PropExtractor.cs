namespace NexusForever.Tool.Extractor.Extract.Prop
{
    public class M3PropExtractor : IPropExtractor
    {
        #region Dependency Injection

        private readonly FileExtractor fileExtractor;

        public M3PropExtractor(
            FileExtractor fileExtractor)
        {
            this.fileExtractor = fileExtractor;
        }

        #endregion

        public void Extract(string propPath)
        {
            fileExtractor.Extract(propPath, out string _);
        }
    }
}
