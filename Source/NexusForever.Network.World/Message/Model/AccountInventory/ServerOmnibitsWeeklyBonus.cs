using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerOmnibitsWeeklyBonus)]
    public class ServerOmnibitsWeeklyBonus : IWritable
    {
        public uint OmnibitsWeeklylBonusEarned { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(OmnibitsWeeklylBonusEarned);
        }
    }
}
