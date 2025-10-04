using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChatAccept)]
    public class ServerChatAccept : IWritable
    {
        public ushort ChatMessageId { get; set; }
        public string Name { get; set; }
        public string RealmName { get; set; }
        public uint Guid { get; set; }
        public bool GM { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ChatMessageId);
            writer.Write(GM);
            writer.Write(0, 5u); // Item count

            writer.WriteStringWide(Name);
            writer.WriteStringWide(RealmName);

            writer.Write(Guid);
            writer.Write(1, 8u); // CharacterId
        }
    }
}
