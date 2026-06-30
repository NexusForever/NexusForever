using CommandLine;
using CommandLine.Text;

namespace NexusForever.TblToCsv;

public class Parameters
{
    [Option('i', "input", Required = true, HelpText = "Folder containing .tbl and .bin game data files to convert.")]
    public string InputPath { get; set; }

    [Option('o', "output", Required = false, Default = "Output",
        HelpText = "Destination folder for the generated .csv files. Defaults to ./Output.")]
    public string OutputPath { get; set; }

    [Usage(ApplicationAlias = "NexusForever.TblToCsv")]
    public static IEnumerable<Example> Examples =>
    [
        new("Convert every .tbl/.bin file in a folder to CSV, written to ./Output",
            new Parameters { InputPath = "C:/Game/Data/Tables" }),
        new("Convert to a specific output folder",
            new Parameters { InputPath = "C:/Game/Data/Tables", OutputPath = "C:/Export" })
    ];
}
