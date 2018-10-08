using NexusForever.Shared.Network.Message;

namespace NexusForever.Shared.Network.Packet
{
    public abstract class GamePacket
    {
        public const ushort HeaderSize = sizeof(uint) + sizeof(ushort);

        /// <summary>
        /// Total size including the header and payload.
        /// </summary>
        public uint Size { get; protected set; }
        public GameMessageOpcode Opcode { get; protected set; }

        public byte[] Data { get; protected set; }
    }
}
