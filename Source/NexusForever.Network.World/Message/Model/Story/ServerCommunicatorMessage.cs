using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerCommunicatorMessage)]
    public class ServerCommunicatorMessage : IWritable
    {
        public ushort CommunicatorMessagesId { get; set; }
        public bool CheckConditions { get; set; } // If false, this will make the message appear even if the player doesn't meet the
                                                  // conditions for the message

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CommunicatorMessagesId, 15u);
            writer.Write(CheckConditions);
        }
    }
}
