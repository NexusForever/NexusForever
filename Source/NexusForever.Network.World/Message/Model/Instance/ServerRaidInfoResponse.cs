using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Instance
{
    [Message(GameMessageOpcode.ServerRaidInfoResponse)]
    public class ServerRaidInfoResponse : IWritable
    {
        public class RaidInfo : IWritable
        {
            public ulong SavedInstanceId { get; set; }
            public ushort WorldId { get; set; }
            public ulong DateExpireUTC { get; set; } // Full date the lock resets
            public float DaysUntilExpire { get; set; } // Relative time from now the lock resests
            public uint PrimeLevel { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(SavedInstanceId);
                writer.Write(WorldId, 15u);
                writer.Write(DateExpireUTC);
                writer.Write(DaysUntilExpire);
                writer.Write(PrimeLevel);
            }
        }

        List<RaidInfo> Raids { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Raids.Count);
            Raids.ForEach(raid => raid.Write(writer));
        }
    }
}
