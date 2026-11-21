using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Story;

namespace NexusForever.Network.World.Message.Model.Story.Message
{
    public sealed class PlayerActor : Actor
    {
        public override StoryTextSourceType Type => StoryTextSourceType.Player;
        public uint UnitId { get; set; }
        public string Name { get; set; }
        public uint Level { get; set; }
        public Sex Gender { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Faction Faction { get; set; }
        public Game.Static.PlayerPath.Path Path { get; set; }
        public ushort TitleId { get; set; }

        public override void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.WriteStringWide(Name);
            writer.Write(Level);
            writer.Write(Gender, 2u);
            writer.Write(Race, 5u);
            writer.Write(Class, 5u);
            writer.Write(Faction, 14u);
            writer.Write(Path, 3u);
            writer.Write(TitleId, 14u);
            base.Write(writer);
        }
    }
}
