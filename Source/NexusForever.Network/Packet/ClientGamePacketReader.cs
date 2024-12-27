using NexusForever.Cryptography;
using NexusForever.Network.Message;

namespace NexusForever.Network.Packet
{
    public sealed class ClientGamePacketReader : IDisposable
    {
        private MemoryStream stream;
        private GamePacketReader reader;

        public void Dispose()
        {
            stream?.Dispose();
            reader?.Dispose();
        }

        public void Initialise(ClientGamePacket packet, PacketCrypt encryption)
        {
            if (stream != null || reader != null)
                throw new InvalidOperationException();

            stream = new MemoryStream(packet.IsEncrypted ? encryption.Decrypt(packet.Data, packet.Data.Length) : packet.Data);
            reader = new GamePacketReader(stream);
        }

        public GameMessageOpcode ReadHeader()
        {
            return reader.ReadEnum<GameMessageOpcode>(16u);
        }

        public uint ReadBody(IReadable message)
        {
            message.Read(reader);
            return reader.BytesRemaining;
        }
    }
}
