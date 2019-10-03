using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Achievement : IWritable
    {
        public ushort AchievementId { get; set; }
        public uint Data0 { get; set; }
        public uint Data1 { get; set; }
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
