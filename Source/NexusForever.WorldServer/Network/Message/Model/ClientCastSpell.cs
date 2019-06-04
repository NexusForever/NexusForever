using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCastSpell)]
    public class ClientCastSpell : IReadable
    {
        public uint ClientUniqueId { get; private set; }
        public ushort BagIndex { get; private set; }
        public uint CasterId { get; private set; }
        public bool ButtonPressed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId = reader.ReadUInt();
            BagIndex  = reader.ReadUShort();
            CasterId  = reader.ReadUInt();
            ButtonPressed  = reader.ReadBit();
        }
    }
}
