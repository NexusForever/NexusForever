using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
