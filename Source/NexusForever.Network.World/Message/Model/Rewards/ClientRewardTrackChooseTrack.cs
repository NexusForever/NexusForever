using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientRewardTrackChooseTrack)]
    public class ClientRewardTrackChooseTrack : IReadable
    {
        public ushort RewardTrackId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RewardTrackId = reader.ReadUShort(14u);
        }
    }
}
