using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NexusForever.Shared.Configuration
{
    public class ConnectionStringConverter : JsonConverter<IConnectionString>
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, IConnectionString value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IConnectionString ReadJson(JsonReader reader, Type objectType, IConnectionString existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);
            if(jobj.Type == JTokenType.Null || jobj.Type == JTokenType.Undefined) return null;
            DatabaseProvider provider = DatabaseProvider.MySql;
            if (jobj.TryGetValue("provider", StringComparison.OrdinalIgnoreCase, out _))
            {
                // if provider is specified, we can stop here and just deserialize. It will fail if provider isn't valid.
                // But that's not our problem.
                return jobj.ToObject<DatabaseConnectionString>();
            } 
            if (jobj.Properties().Any(i => string.Equals(i.Name, "Host")))
            {
                // This must be the existing configuration.
                return jobj.ToObject<MySqlConfig>();
            }
            // Assume it's MSSQL, since MySql didn't use this format, and provider really should be explicit anyway.
            var ret = jobj.ToObject<DatabaseConnectionString>();
            ret.Provider = DatabaseProvider.MicrosoftSqlServer;
            return ret;
        }
    }
}