using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventPersonalStatUpdate)]
    public class ServerPublicEventPersonalStatUpdate : IWritable
    {
        public uint EventId { get; set; }
        public PublicEventStat StatType { get; set; }
        public int Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EventId, 14u);
            writer.Write(StatType, 32u);
            writer.Write(Value);
        }
    }
}
