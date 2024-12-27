using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class PublicEventTeamStats : IWritable
    {
        public PublicEventTeam TeamId { get; set; }
        public PublicEventStats Stats { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TeamId, 14);
            Stats.Write(writer);
        }
    }
}
