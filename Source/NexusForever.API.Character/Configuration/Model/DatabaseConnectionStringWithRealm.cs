using NexusForever.Database.Configuration.Model;

namespace NexusForever.API.Character.Configuration.Model
{
    public class DatabaseConnectionStringWithRealm : DatabaseConnectionString
    {
        public ushort RealmId { get; set; }
    }
}
