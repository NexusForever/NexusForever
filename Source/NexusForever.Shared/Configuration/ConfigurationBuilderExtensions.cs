using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace NexusForever.Shared.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddNexusForeverJson(this IConfigurationBuilder builder, string path)
        {
            if (Environment.GetEnvironmentVariable("NEXUSFOREVER_ASPIRE") == "1"
                && !builder.GetFileProvider().GetFileInfo(path).Exists)
                path = Path.ChangeExtension(path, "example.json");

            return builder.AddJsonFile(path, optional: false);
        }
    }
}
