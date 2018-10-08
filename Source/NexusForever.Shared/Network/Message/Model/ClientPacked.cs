namespace NexusForever.Shared.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPacked, MessageDirection.Client)]
    public class ClientPacked : IReadable
    {
        public byte[] Data { get; private set; }

        public void Read(GamePacketReader reader)
        {
            uint length = reader.ReadUInt();
            Data = reader.ReadBytes(length - 4);
        }
    }
}
