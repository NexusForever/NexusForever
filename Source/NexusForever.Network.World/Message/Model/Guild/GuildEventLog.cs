using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    public class GuildEventLog : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public ulong EventLogId { get; set; }
        public GuildEventType EventType { get; set; }
        public ulong CharacterId { get; set; }
        public ulong Item2Id { get; set; }
        public ulong Amount { get; set; } // Can be money amount or item count for item operations
        public ulong Unknown { get; set; }
        public float DaysAgoOccured { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(EventLogId);
            writer.Write(EventType, 17u);
            writer.Write(CharacterId);
            writer.Write(Item2Id);
            writer.Write(Amount);
            writer.Write(Unknown);
            writer.Write(DaysAgoOccured);
        }
    }
}
