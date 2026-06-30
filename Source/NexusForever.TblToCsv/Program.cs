using CommandLine;
using CommandLine.Text;
using NexusForever.GameTable;
using NexusForever.TblToCsv;

using var parser = new Parser(settings => settings.HelpWriter = null);
ParserResult<Parameters> parserResult = parser.ParseArguments<Parameters>(args);
parserResult
    .WithParsed(Run)
    .WithNotParsed(errs => DisplayHelp(parserResult, errs));

static void DisplayHelp(ParserResult<Parameters> result, IEnumerable<Error> errs)
{
    HelpText helpText = HelpText.AutoBuild(result, h =>
    {
        h.Heading = "NexusForever.TblToCsv";
        h.Copyright = string.Empty;
        h.AddPreOptionsLine(
            "\nConverts WildStar .tbl and .bin game data files to .csv, one file per table.\n" +
            "All reading is done with NexusForever's own GameTable<T>/TextTable loaders\n" +
            "(NexusForever.GameTable) - this program does not parse the binary format itself.\n" +
            "\nThe only custom logic is a small reflection-based parser that flattens the\n" +
            "strongly-typed entries returned by GameTable<T> (e.g. arrays declared with\n" +
            "[GameTableFieldArray]) into generic, indexed CSV columns, since the typed\n" +
            "loader returns C# objects rather than flat rows.\n" +
            "\n.tbl files are matched against GameTableManager's [GameData] properties by\n" +
            "filename; .tbl files with no matching entry type are skipped.");
        return h;
    }, e => e);

    Console.Error.WriteLine(helpText);
    Environment.Exit(1);
}

static void Run(Parameters p)
{
    Directory.CreateDirectory(p.OutputPath);

    // --- .tbl files ---
    Dictionary<string, Type> typeMap = TypedTableReader.BuildTypeMap();

    foreach (string tblPath in Directory.EnumerateFiles(p.InputPath, "*.tbl"))
    {
        string fileName = Path.GetFileName(tblPath);
        string name = Path.GetFileNameWithoutExtension(tblPath);
        string outPath = Path.Combine(p.OutputPath, name + ".csv");

        if (!typeMap.TryGetValue(fileName, out Type entryType))
        {
            Console.WriteLine($"[SKIP] {name}.tbl  no matching GameTableManager entry type");
            continue;
        }

        try
        {
            (string[] columns, string[][] rows) = TypedTableReader.Parse(tblPath, entryType);
            CsvWriter.Write(outPath, columns, rows.Cast<object[]>());
            Console.WriteLine($"[OK]  {name}.tbl  ({rows.Length} rows)");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERR] {name}.tbl  {ex.Message}");
        }
    }

    // --- .bin language files ---
    foreach (string binPath in Directory.EnumerateFiles(p.InputPath, "*.bin"))
    {
        string name = Path.GetFileNameWithoutExtension(binPath);
        string outPath = Path.Combine(p.OutputPath, name + ".csv");

        try
        {
            TextTable table = GameTableFactory.LoadTextTable(binPath);
            if (table == null)
            {
                Console.Error.WriteLine($"[ERR] {name}.bin  file not found or empty");
                continue;
            }

            var columns = new[] { "Id", "Language", "Text" };
            IEnumerable<object[]> rows = table.Entries.Select(e => new object[]
            {
                e.Id,
                e.Language.ToString(),
                e.Text ?? string.Empty
            });

            CsvWriter.Write(outPath, columns, rows);
            Console.WriteLine($"[OK]  {name}.bin  ({table.Entries.Length} entries)");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERR] {name}.bin  {ex.Message}");
        }
    }
}
