using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCastSpellContinuous)]
    public class ClientCastSpellContinuous : IReadable
    {
        public ushort BagIndex { get; private set; }
        public uint Guid { get; private set; }
        public bool ButtonPressed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BagIndex  = reader.ReadUShort();
            Guid      = reader.ReadUInt();
            ButtonPressed = reader.ReadBit();
        }
    }
}
