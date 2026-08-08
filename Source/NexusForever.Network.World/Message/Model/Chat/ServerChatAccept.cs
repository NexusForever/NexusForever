using NexusForever.Network.Message;
using NexusForever.Network.World.Chat.Model;

namespace NexusForever.Network.World.Message.Model.Chat
{
    // Sent back to the client after the client sends any chat message. May send item details for the chat message.
    // If there was an item linked, the client used this data to go through the previous message specifed by
    // ChatMesssageId and replaces any Formats of type ItemGuid with ItemFull.

    [Message(GameMessageOpcode.ServerChatAccept)]
    public class ServerChatAccept : IWritable
    {
        public ushort ChatMessageId { get; set; } // used to match the original ClientChat message
        public bool GM { get; set; }
        public List<ChatFormatItemFull> Formats { get; } = [];
        public string SenderName { get; set; }
        public string RealmName { get; set; }
        public uint UnitId { get; set; }
        public byte PremiumTier { get; set; } = 1;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChatMessageId);
            writer.Write(GM);
            
            writer.Write(Formats.Count, 5u);
            Formats.ForEach(format => format.Write(writer));

            writer.WriteStringWide(SenderName);
            writer.WriteStringWide(RealmName);

            writer.Write(UnitId);
            writer.Write(PremiumTier,8u);
        }
    }
}
