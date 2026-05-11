using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatResult)]
    public class ServerChatResult : IWritable
    {
        public Channel Channel { get; set; }
        public ChatResult ChatResult { get; set; }
        public ushort ChatMessageId { get; set; } // removes the message from the chat log with this id

        public void Write(GamePacketWriter writer)
        {
            Channel.Write(writer);
            writer.Write(ChatResult, 5u);
            writer.Write(ChatMessageId);
        }
    }
}
