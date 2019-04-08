namespace NexusForever.Shared.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientPacked)]
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
