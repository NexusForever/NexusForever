using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventLeave)]
    public class ServerPublicEventLeave : IWritable
    {
        public uint PublicEventId { get; set; }
        public PublicEventRemoveReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PublicEventId, 14);
            writer.Write(Reason, 32);
        }
    }
}
