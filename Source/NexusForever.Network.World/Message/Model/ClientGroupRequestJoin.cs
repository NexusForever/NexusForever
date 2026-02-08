using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupRequestJoin)]
    public class ClientGroupRequestJoin : IReadable
    {
        public string Name { get; set; }
        public string RealmName { get; set; }

        public void Read(GamePacketReader reader)
        {
            Name      = reader.ReadWideString();
            RealmName = reader.ReadWideString();
        }
    }
}
