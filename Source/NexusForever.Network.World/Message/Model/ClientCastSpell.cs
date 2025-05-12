using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCastSpell)]
    public class ClientCastSpell : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; } // first value of 0x7FD response, probably global increment
        public ushort BagIndex { get; private set; }
        public uint TargetUnitId { get; private set; }
        public bool ButtonPressed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            BagIndex  = reader.ReadUShort();
            TargetUnitId = reader.ReadUInt();
            ButtonPressed  = reader.ReadBit();
        }
    }
}
