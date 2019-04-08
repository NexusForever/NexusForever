namespace NexusForever.Shared.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEncrypted)]
    public class ClientEncrypted : IReadable
    {
        public byte[] Data { get; private set; }

        public void Read(GamePacketReader reader)
        {
            uint length = reader.ReadUInt();
            Data = reader.ReadBytes(length - 4);
        }
    }
}
