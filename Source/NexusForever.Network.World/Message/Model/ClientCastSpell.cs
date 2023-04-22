using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCastSpell)]
    public class ClientCastSpell : IReadable
    {
        public uint ClientUniqueId { get; private set; } // first value of 0x7FD response, probably global increment
        public ushort BagIndex { get; private set; }
        public uint CasterId { get; private set; }
        public bool ButtonPressed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientUniqueId  = reader.ReadUInt();
            BagIndex  = reader.ReadUShort();
            CasterId  = reader.ReadUInt();
            ButtonPressed  = reader.ReadBit();
        }
    }
}
