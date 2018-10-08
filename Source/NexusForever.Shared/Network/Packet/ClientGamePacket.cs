using System.IO;
using NexusForever.Shared.Network.Message;

namespace NexusForever.Shared.Network.Packet
{
    public class ClientGamePacket : GamePacket
    {
        public ClientGamePacket(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new GamePacketReader(stream))
            {
                Opcode = (GameMessageOpcode)reader.ReadUShort();
                Data   = reader.ReadBytes(reader.BytesRemaining);
                Size   = (uint)Data.Length + HeaderSize;
            }
        }
    }
}
