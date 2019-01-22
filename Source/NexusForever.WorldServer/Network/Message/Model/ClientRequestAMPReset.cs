using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRequestAMPReset, MessageDirection.Client)]
    public class ClientRequestAMPReset : IReadable
    {
        public byte ActionSetIndex { get; private set; }
        public byte ResetType { get; private set; }
        public uint Value { get; private set; }
        

        public void Read(GamePacketReader reader)
        {
            ActionSetIndex = reader.ReadByte(3u);
            ResetType = reader.ReadByte(3u);
            Value = reader.ReadUInt();
        }
    }
}
