using System.Globalization;
using System.Reflection;
using NexusForever.GameTable;

namespace NexusForever.TblToCsv;

/// <summary>
/// all binary reading is delegated to
/// NexusForever.GameTable's <see cref="GameTable{T}"/>/<see cref="GameTableFactory"/>.
/// This class only adapts the strongly-typed entries that loader returns into flat,
/// generic columns/rows suitable for CSV output, since GameTable<T>; exposes C#
/// objects (with array fields) rather than tabular data.
/// </summary>
public static class TypedTableReader
{
    private static Dictionary<string, Type> _typeMap;

    /// <summary>
    /// Builds a map from .tbl filename (case-insensitive) to entry Type by reflecting
    /// over all GameTable&lt;T&gt; properties on GameTableManager.
    /// </summary>
    public static Dictionary<string, Type> BuildTypeMap()
    {
        if (_typeMap != null)
            return _typeMap;

        _typeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo prop in typeof(GameTableManager).GetProperties())
        {
            if (!prop.PropertyType.IsGenericType)
                continue;
            if (prop.PropertyType.GetGenericTypeDefinition() != typeof(GameTable<>))
                continue;

            // Resolve filename: explicit [GameData("...")] wins, else PropertyName.tbl
            string fileName;
            GameDataAttribute attr = prop.GetCustomAttribute<GameDataAttribute>();
            if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
                fileName = Path.GetFileName(attr.FileName);
            else
                fileName = prop.Name + ".tbl";

            Type entryType = prop.PropertyType.GetGenericArguments()[0];
            _typeMap[fileName] = entryType;
        }

        return _typeMap;
    }

    /// <summary>
    /// Loads a .tbl file using NF's typed parser and returns flat string rows.
    /// </summary>
    public static (string[] Columns, string[][] Rows) Parse(string filePath, Type entryType)
    {
        object table = GameTableFactory.LoadGameTable(entryType, filePath);
        Array entries = (Array)table.GetType().GetProperty("Entries").GetValue(table);

        FieldInfo[] fields = entryType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        // Build column names
        var columns = new List<string>();
        foreach (FieldInfo field in fields)
        {
            GameTableFieldArrayAttribute arrAttr = field.GetCustomAttribute<GameTableFieldArrayAttribute>();
            if (arrAttr != null)
                for (uint i = 0; i < arrAttr.Length; i++)
                    columns.Add($"{field.Name}{i:D2}");
            else
                columns.Add(field.Name);
        }

        // Build rows
        var rows = new string[entries.Length][];
        for (int i = 0; i < entries.Length; i++)
        {
            object entry = entries.GetValue(i);
            var values = new List<string>();

            foreach (FieldInfo field in fields)
            {
                GameTableFieldArrayAttribute arrAttr = field.GetCustomAttribute<GameTableFieldArrayAttribute>();
                if (arrAttr != null)
                {
                    var arr = (Array)field.GetValue(entry);
                    for (int j = 0; j < (int)arrAttr.Length; j++)
                        values.Add(FormatValue(arr?.GetValue(j)));
                }
                else
                {
                    values.Add(FormatValue(field.GetValue(entry)));
                }
            }

            rows[i] = values.ToArray();
        }

        return (columns.ToArray(), rows);
    }

    public static string FormatValue(object value)
    {
        return value switch
        {
            null        => string.Empty,
            bool b      => b ? "True" : "False",
            float f     => f.ToString("G9", CultureInfo.InvariantCulture),
            string s    => s,
            _ when value.GetType().IsEnum
                        => Convert.ToUInt32(value).ToString(CultureInfo.InvariantCulture),
            _           => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }
}
