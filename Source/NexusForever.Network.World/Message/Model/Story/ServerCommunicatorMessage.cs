using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerCommunicatorMessage)]
    public class ServerCommunicatorMessage : IWritable
    {
        public ushort CommunicatorMessagesId { get; set; }

        /// <summary>
        /// True: the client re-runs the row's conditions on a message without a delay; delayed messages are always
        /// re-checked when the delay expires (client 0x14043c3f0). If false, the message appears even if the player doesn't
        /// meet the conditions for the message. Rows without flags bit 0 are dropped either way (0x14043b7d3).
        /// </summary>
        public bool CheckConditions { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CommunicatorMessagesId, 15u);
            writer.Write(CheckConditions);
        }
    }
}
