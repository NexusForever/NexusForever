using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    public class Achievement : IWritable
    {
        public ushort AchievementId { get; set; }
        public uint ChecklistFlagsLow { get; set; } // for achievement objectives with index of 0 to 31
        public uint ChecklistFlagsHigh { get; set; } // for achievement objectives with index of 32 to 63
        public ulong DateCompleted { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AchievementId, 15u);
            writer.Write(ChecklistFlagsLow);
            writer.Write(ChecklistFlagsHigh);
            writer.Write(DateCompleted);
        }
    }
}
