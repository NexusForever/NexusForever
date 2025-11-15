namespace NexusForever.Network.Message.Model
{
    // Only used for ClientCharacterCreate (0x25B)
    // Adds a layer of wrapping around the 0x25B message but doesn't do anything else
    // and it then is data protected and wrapped inside a ClientEncrypted message (0x244)
    [Message(GameMessageOpcode.ClientCharacterCreatePacked)]
    public class ClientCharacterCreatePacked : IReadable
    {
        public byte[] Data { get; private set; }

        public void Read(GamePacketReader reader)
        {
            uint length = reader.ReadUInt();
            Data = reader.ReadBytes(length - 4);
        }
    }
}
