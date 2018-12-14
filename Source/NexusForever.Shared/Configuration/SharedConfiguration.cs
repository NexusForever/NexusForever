using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace NexusForever.Shared.Configuration
{
    public static class SharedConfiguration
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public static IConfiguration Configuration { get; private set; }

        public static void Initialize(string file)
        {
            if (Configuration != null)
                return;

            MigrateDatabaseConfiguration(file);

            var builder = new ConfigurationBuilder();

            builder
                .AddJsonFile(file, false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
        }

        // The code below this line should be removed after this reaches master for a few releases/weeks.
        // This is only meant to aide people in updating their configs!
        private static void MigrateDatabaseConfiguration(string file)
        {
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
            JToken value = jsonObject.SelectToken("Database.Auth.Host") ??
                           jsonObject.SelectToken("Database.Character.Host") ??
                           jsonObject.SelectToken("Database.World.Host");
            if (value == null)
            {
                log.Debug("Not migrating configuration, Host property not found on any database configuration.");
                return;
            }

            File.WriteAllText($"{file}.bak", File.ReadAllText(file));

            log.Warn("Configuration format has changed, and your configuration is being migrated to the new format");
            log.Warn("A backup of your configuration has been saved to {0}.bak", file);

            ConvertSingleDatabaseSection("Database.Auth", jsonObject);
            ConvertSingleDatabaseSection("Database.Character", jsonObject);
            ConvertSingleDatabaseSection("Database.World", jsonObject);

            File.WriteAllText(file, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
            log.Warn("Updated configuration written to {0}", file);
        }

        private static void ConvertSingleDatabaseSection(string path, JObject root)
        {
            var selectedToken = root.SelectToken(path) as JObject;
            if (selectedToken == null)
            {
                log.Debug("Skipping non-existent section {0}", path);
                return;
            }

            if (selectedToken.Property("Host") == null)
            {
                log.Debug(
                    "Not migrating configuration at path: {0}, because no host property exists. Already converted?",
                    path);
                return;
            }

            // properties to read
            string[] originalProperties =
            {
                "Host",
                "Port",
                "Username",
                "Password",
                "Database"
            };

            log.Debug("Reading old configuration format for JSON Path: {0}", path);

            List<string> parts = originalProperties
                .Select(propertyName => selectedToken.Value<string>(propertyName))
                .ToList();
            string connectionString =
                $"server={parts[0]};port={parts[1]};user={parts[2]};password={parts[3]};database={parts[4]}";

            selectedToken.RemoveAll();

            log.Debug("Creating new configuration at JSON Path: {0}", path);

            selectedToken.Add("ConnectionString", JValue.CreateString(connectionString));
            selectedToken.Add("Provider", JValue.CreateString(DatabaseProvider.MySql.ToString()));
        }
    }
}
