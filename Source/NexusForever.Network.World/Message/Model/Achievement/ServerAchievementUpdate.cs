using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Achievement
{
    [Message(GameMessageOpcode.ServerAchievementUpdate)]
    public class ServerAchievementUpdate : IWritable
    {
        public bool Deleted { get; set; }
        public List<Achievement> Achievements { get; set; } = []; 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Deleted);
            writer.Write(Achievements.Count);
            Achievements.ForEach(a => a.Write(writer));
        }
    }
}
