using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientReplayLevelRequest)]
    public class ClientReplayLevelUp : IReadable
    {
        public uint Level { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Level = reader.ReadUInt();
        }
    }
}
