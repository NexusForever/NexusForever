using NexusForever.Network.Message;

namespace NexusForever.Network.Packet
{
    public class ServerGamePacket
    {
        public const ushort HeaderSize = sizeof(uint) + sizeof(ushort);

        /// <summary>
        /// Total size including the header and payload.
        /// </summary>
        public uint Size { get; protected set; }
        public GameMessageOpcode Opcode { get; protected set; }
        public byte[] Data { get; protected set; }

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
