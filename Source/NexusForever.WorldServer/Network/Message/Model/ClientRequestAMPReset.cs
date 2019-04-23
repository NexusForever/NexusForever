using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRequestAmpReset)]
    public class ClientRequestAmpReset : IReadable
    {
        public byte ActionSetIndex { get; private set; }
        public AmpRespecType RespecType { get; private set; }
        public uint Value { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ActionSetIndex = reader.ReadByte(3u);
            RespecType     = reader.ReadEnum<AmpRespecType>(3u);
            Value          = reader.ReadUInt();
        }
    }
}
