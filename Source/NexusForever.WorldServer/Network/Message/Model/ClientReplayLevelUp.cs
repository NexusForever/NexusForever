using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientReplayLevelRequest)]
    public class ClientReplayLevelUp : IReadable
    {
        public uint Level { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Level  = reader.ReadUInt();
        }
    }
}
