using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChatResult)]
    public class ServerChatResult : IWritable
    {
        public Channel Channel { get; set; }
        public ChatResult ChatResult { get; set; }
        public ushort Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.Write(ChatResult, 5u);
            writer.Write(Unknown0);
        }
    }
}
