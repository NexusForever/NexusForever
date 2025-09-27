using NexusForever.Network.Message;

namespace NexusForever.Network.Auth.Message.Model
{
    [Message(GameMessageOpcode.ServerSubscriptionInfo)]
    public class ServerSubscriptionInfo : IWritable
    {
        [Flags]
        public enum RealmTransferFlags
        {
            FreeRealmTransfer = 0x1,
        }

        public RealmTransferFlags Flags { get; set; }
        public bool SubscriptionExpired { get; set; }
        public float GameTimeHoursRemaining { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Flags, 32u);
            writer.Write(SubscriptionExpired);
            writer.Write(GameTimeHoursRemaining);
        }
    }
}
