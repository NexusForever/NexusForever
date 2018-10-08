using System.IO;
using NexusForever.Shared.Network.Message;

namespace NexusForever.Shared.Network.Packet
{
    public class ServerGamePacket : GamePacket
    {
        public ServerGamePacket(GameMessageOpcode opcode, IWritable message)
        {
            using (var stream = new MemoryStream())
            using (var writer = new GamePacketWriter(stream))
            {
                message.Write(writer);
                writer.FlushBits();
                Data = stream.ToArray();
            }

            Opcode = opcode;
            Size   = (ushort)(HeaderSize + Data.Length);
        }
    }
}
