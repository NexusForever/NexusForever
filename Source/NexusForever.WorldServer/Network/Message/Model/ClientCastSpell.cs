using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCastSpell, MessageDirection.Client)]
    public class ClientCastSpell : IReadable
    {
        public ushort BagIndex { get; private set; }
        public uint Guid { get; private set; }
        public bool Unknown48 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BagIndex  = reader.ReadUShort();
            Guid      = reader.ReadUInt();
            Unknown48 = reader.ReadBit();
        }
    }
}
