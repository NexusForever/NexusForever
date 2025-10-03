using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    [Message(GameMessageOpcode.ServerRealmFirstAchievement)]
    public class ServerRealmFirstAchievement : IWritable
    {
        public ushort AchievementId { get; set; }
        public bool IsGuildAchievement { get; set; }
        public string Name { get; set; } // player or guild name

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AchievementId, 15u);
            writer.Write(IsGuildAchievement);
            writer.WriteStringWide(Name);
        }
    }
}
