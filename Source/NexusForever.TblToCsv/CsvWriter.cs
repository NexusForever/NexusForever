using System.Globalization;
using System.Text;

namespace NexusForever.TblToCsv;

public static class CsvWriter
{
    public static void Write(string path, IEnumerable<string> columns, IEnumerable<object[]> rows)
    {
        using var sw = new StreamWriter(path, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        sw.WriteLine(string.Join(",", columns.Select(Escape)));

        foreach (object[] row in rows)
            sw.WriteLine(string.Join(",", row.Select(FormatValue).Select(Escape)));
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            null        => string.Empty,
            bool b      => b ? "True" : "False",
            float f     => f.ToString("G9", CultureInfo.InvariantCulture),
            double d    => d.ToString("G17", CultureInfo.InvariantCulture),
            _           => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
