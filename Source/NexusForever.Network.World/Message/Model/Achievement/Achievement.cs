using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    public class Achievement : IWritable
    {
        // TODO: Determine the data types for the Data0/Data1 fields for each achievement. This is based on the achievementTypeId

        public ushort AchievementId { get; set; } 
        public uint Data0 { get; set; } // simply counted type: uint number
                                        // checklist type: flags for achievement objectives with index of 0 to 31
        public uint Data1 { get; set; } // checklist type: flags for achievement objectives with index of 32 to 63
        public ulong DateCompleted { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AchievementId, 15u);
            writer.Write(Data0);
            writer.Write(Data1);
            writer.Write(DateCompleted);
        }
    }
}
