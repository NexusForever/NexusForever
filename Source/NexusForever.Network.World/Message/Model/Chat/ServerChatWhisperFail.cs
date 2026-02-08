using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Chat
{
    [Message(GameMessageOpcode.ServerChatWhisperFail)]
    public class ServerChatWhisperFail : IWritable
    {
        public string CharacterTo { get; set; }
        public bool IsAccountWhisper { get; set; }
        public ushort ChatMessageId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(CharacterTo);
            writer.Write(IsAccountWhisper);
            writer.Write(ChatMessageId);
        }
    }
}
