using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.AccountInventory
{
    [Message(GameMessageOpcode.ClientDailyLoginRewardsRequest)]
    public class ClientDailyLoginRewardsRequest : IReadable
    {
        public void Read(GamePacketReader reader)
        {
            // zero byte message
        }
    }
}
