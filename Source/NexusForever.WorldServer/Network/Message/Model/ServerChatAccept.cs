using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChatAccept)]
    class ServerChatAccept : IWritable
    {
        public string Name { get; set; }
        public string RealmName { get; set; }
        public uint Guid { get; set; }
        public bool GM { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(1, 16u); // Result?
            writer.Write(GM);
            writer.Write(0, 5u); // Item count

            writer.WriteStringWide(Name);
            writer.WriteStringWide(RealmName);

            writer.Write(Guid);
            writer.Write(1, 8u); // CharacterId
        }
    }
}
