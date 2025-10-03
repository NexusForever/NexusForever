using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    [Message(GameMessageOpcode.ServerAchievementInit)]
    public class ServerAchievementInit : IWritable
    {
        public List<Achievement> Achievements { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Achievements.Count);
            Achievements.ForEach(a => a.Write(writer));
        }
    }
}
