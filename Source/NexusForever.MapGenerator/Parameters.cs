using CommandLine;

namespace NexusForever.MapGenerator
{
    public class Parameters
    {
        [Option('i', "patchPath", Required = true,
            HelpText = "The location of the WildStar client patch folder.")]
        public string PatchPath { get; set; }

        [Option('e', "extract",
            HelpText = "Extract all game tables (.tbl) and localisation files (.bin).")]
        public bool Extract { get; set; }

        [Option('g', "generate",
            HelpText = "Generate base map files (.nfmap) from the area files.")]
        public bool Generate { get; set; }

        [Option("debug")]
        public bool Debug { get; set; }

        // generation specific options
        [Option("worldId")]
        public ushort? WorldId { get; set; }

        [Option("gridX")]
        public byte? GridX { get; set; }

        [Option("gridY")]
        public byte? GridY { get; set; }
    }
}
