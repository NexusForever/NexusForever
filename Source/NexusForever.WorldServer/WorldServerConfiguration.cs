using NexusForever.Database.Configuration.Model;
using NexusForever.Game.Configuration.Model;
using NexusForever.GameTable.Configuration.Model;
using NexusForever.Network.Configuration.Model;
using NexusForever.Script.Configuration.Model;

namespace NexusForever.WorldServer
{
    public class WorldServerConfiguration
    {
        public NetworkConfig Network { get; set; }
        public DatabaseConfig Database { get; set; }
        public GameTableConfig GameTable { get; set; }
        public RealmConfig Realm { get; set; }
        public ScriptConfig Script { get; set; }
    }
}
