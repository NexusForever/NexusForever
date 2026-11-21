using NexusForever.Game.Static.Reward;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ClientRewardRotationsRequestUpdate)]
    public class ClientRewardRotationsRequestUpdate : IReadable
    {
        public RewardRotationContentType ContentType { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ContentType = reader.ReadEnum<RewardRotationContentType>(32u);
        }
    }
}
