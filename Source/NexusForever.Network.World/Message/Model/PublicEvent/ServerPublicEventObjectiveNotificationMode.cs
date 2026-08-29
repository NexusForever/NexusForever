using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventObjectiveNotificationMode)]
    public class ServerPublicEventObjectiveNotificationMode : IWritable
    {
        public uint ObjectiveId { get; set; }
        public PublicEventObjectiveNotificationMode NotificationMode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 14);
            writer.Write(NotificationMode, 32u);
        }
    }
}
