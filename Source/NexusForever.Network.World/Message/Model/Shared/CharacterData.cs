using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class CharacterData : IWritable
    {
        public string Name { get; set; } = "";
        public TargetPlayerIdentity PlayerIdentity  { get; set; } = new TargetPlayerIdentity();
        public Class Class { get; set; }
        public Race Race  { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public uint Level { get; set; }
        public ushort WorldZoneId { get; set; }
        public ushort Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Name);
            PlayerIdentity.Write(writer);
            writer.Write(Class, 14u);
            writer.Write(Race, 14u);
            writer.Write(Path, 32);
            writer.Write(Level);
            writer.Write(WorldZoneId, 15u);
            writer.Write(Unknown0, 14u);
        }
    }
}

