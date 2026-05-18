using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGroupInvite)]
    public class ClientGroupInvite : IReadable
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
