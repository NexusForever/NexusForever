using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatJoinResult)]
    public class ServerChatJoinResult : IWritable
    {
        public ChatChannelType ChatChannelId { get; set; }
        public string Name { get; set; }
        public ChatResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChatChannelId, 14u);
            writer.WriteStringWide(Name);
            writer.Write(Result, 5u);
        }
    }
}
