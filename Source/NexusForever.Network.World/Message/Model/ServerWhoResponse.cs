using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerWhoResponse)]
    public class ServerWhoResponse : IWritable
    {
        public class WhoPlayer : IWritable
        {
            public string Name { get; set; }
            public string Realm { get; set; }
            public uint Level { get; set; }
            public Race Race { get; set; }
            public Class Class { get; set; }
            public Path Path { get; set; }
            public Faction Faction { get; set; }
            public Sex Sex { get; set; }
            public uint Zone { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Name);
                writer.WriteStringWide(Realm);
                writer.Write(Level, 32);
                writer.Write(Race, 5);
                writer.Write(Class, 5);
                writer.Write(Sex, 2);
                writer.Write(Faction, 14);
                writer.Write(Path, 3);
                writer.Write(Zone, 15);
            }
        }

        public List<WhoPlayer> Players { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Players.Count, 32);

            Players.ForEach((player) => player.Write(writer));

            writer.Write(1, 2);
        }
    }
}
