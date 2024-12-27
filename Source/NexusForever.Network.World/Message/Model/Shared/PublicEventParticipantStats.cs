using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PublicEventParticipantStats : IWritable
    {
        public PublicEventTeam TeamId { get; set; }
        public uint UnitId { get; set; }
        public TargetPlayerIdentity Player { get; set; }
        public Class Class { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public PublicEventStats Stats { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TeamId, 14);
            writer.Write(UnitId);
            Player.Write(writer);
            writer.Write(Class, 32);
            writer.Write(Path, 32);
            Stats.Write(writer);
        }
    }
}
