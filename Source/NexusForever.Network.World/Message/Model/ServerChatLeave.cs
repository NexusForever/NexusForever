using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
