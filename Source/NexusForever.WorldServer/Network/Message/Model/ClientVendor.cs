using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientVendor)]
    public class ClientVendor : IReadable
    {
        public uint Guid { get; private set; }
        public byte Unknown0 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Guid     = reader.ReadUInt();
            Unknown0 = reader.ReadByte(7);
        }
    }
}
