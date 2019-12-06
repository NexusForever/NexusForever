using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientRewardTrackChoice)]
    public class ClientRewardTrackChoice : IReadable
    {
        public ushort RewardId { get; private set; } // 14
        public uint Index { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RewardId = reader.ReadUShort(14u);
            Index = reader.ReadUInt();
        }
    }
}
