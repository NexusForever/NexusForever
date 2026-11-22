using CommandLine;

namespace NexusForever.Tool.Extractor
{
    public class Parameters
    {
        [Option('p', "patchPath", Required = true,
            HelpText = "The path of the WildStar client patch folder.")]
        public string PatchPath { get; set; }

        [Option('o', "outputPath", Required = true,
            HelpText = "The path where extracted files will be stored.")]
        public string OutputPath { get; set; }

        [Option('e', "extractArea")]
        public bool ExtractArea { get; set; }

        [Option('f', "extractProp")]
        public bool ExtractProp { get; set; }

        [Option('g', "extractGameTable")]
        public bool ExtractGameTable { get; set; }
    }
}
