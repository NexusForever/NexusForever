using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChatJoinResult)]
    public class ServerChatJoinResult : IWritable
    {
        public ChatChannelType Type { get; set; }
        public string Name { get; set; }
        public ChatResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 14u);
            writer.WriteStringWide(Name);
            writer.Write(Result, 5u);
        }
    }
}
