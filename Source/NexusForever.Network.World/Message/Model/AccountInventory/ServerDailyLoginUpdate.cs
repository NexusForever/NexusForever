using NexusForever.Game.Static.AccountInventory;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerDailyLoginUpdate)]
    public class ServerDailyLoginUpdate : IWritable
    {
        public uint LoginDaysTotal { get; set; }
        public uint Unknown1 { get; set; }
        public uint CurrentLoginDays { get; set; }
        public uint Unknown2 { get; set; }
        public uint NextDailyRewardNotifyTime { get; set; } // relative time from now in milliseconds
        public float NextLockboxKeyTimeInDays { get; set; }
        public PremiumLockboxKeyStatus PremiumKeyStatus { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LoginDaysTotal);
            writer.Write(Unknown1);
            writer.Write(CurrentLoginDays);
            writer.Write(Unknown2);
            writer.Write(NextDailyRewardNotifyTime);
            writer.Write(NextLockboxKeyTimeInDays);
            writer.Write(PremiumKeyStatus, 3u);
        }
    }
}
