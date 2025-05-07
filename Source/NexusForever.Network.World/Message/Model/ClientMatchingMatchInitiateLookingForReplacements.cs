using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientMatchingMatchInitiateLookingForReplacements)]
    public class ClientMatchingMatchInitiateLookingForReplacements : IReadable
    {
        public uint Unknown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown = reader.ReadUInt();
        }
    }
}
