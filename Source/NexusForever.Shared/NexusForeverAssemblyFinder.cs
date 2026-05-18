using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NexusForever.Shared;

public static class NexusForeverAssemblyHelper
{
    /// <summary>
    /// Returns a collection of NexusForever assemblies.
    /// </summary>
    /// <remarks>
    /// Finds any NexusForever assembly in the executing assembly path, loading and returning to the caller.<br/>
    /// This is not a fast implementation and should only be used during initialisation.
    /// </remarks>
    public static IEnumerable<Assembly> GetAssemblies(string searchPattern = "NexusForever")
    {
        string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        foreach (string file in Directory.EnumerateFiles(directory, $"{searchPattern}*dll"))
            yield return Assembly.LoadFrom(file);
    }
}
