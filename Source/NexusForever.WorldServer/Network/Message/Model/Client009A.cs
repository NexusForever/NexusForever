using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Client009A)]
    public class Client009A : IReadable
    {
        public uint Unknown0 { get; private set; } // first value of 0x7FD response, probably global increment
        public ushort BagIndex { get; private set; }
        public uint CasterId { get; private set; }
        public bool Unknown1 { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0  = reader.ReadUInt();
            BagIndex  = reader.ReadUShort();
            CasterId  = reader.ReadUInt();
            Unknown1  = reader.ReadBit();
        }
    }
}
