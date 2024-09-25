using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPublicEventObjectiveStatusUpdate)]
    public class ServerPublicEventObjectiveStatusUpdate : IWritable
    {
        public uint ObjectiveId { get; set; }
        public PublicEventObjectiveStatus ObjectiveStatus { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ObjectiveId, 15);
            ObjectiveStatus.Write(writer);
        }
    }
}
