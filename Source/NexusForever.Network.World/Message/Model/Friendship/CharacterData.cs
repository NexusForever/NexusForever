using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Friendship
{
    public class CharacterData : IWritable
    {
        public string Name { get; set; } = "";
        public Identity PlayerIdentity  { get; set; } = new();
        public Class Class { get; set; }
        public Race Race  { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public uint Level { get; set; }
        public ushort WorldZoneId { get; set; }
        public Faction Faction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Name);
            PlayerIdentity.Write(writer);
            writer.Write(Class, 14u);
            writer.Write(Race, 14u);
            writer.Write(Path, 32);
            writer.Write(Level);
            writer.Write(WorldZoneId, 15u);
            writer.Write(Faction, 14u);
        }
    }
}

