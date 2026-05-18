using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatLeave)]
    public class ServerChatLeave : IWritable
    {
        public Channel Channel { get; set; }
        public ChatChannelLeaveReason Leave { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.Write(Leave, 2u);
        }
    }
}
