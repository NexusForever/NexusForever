using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAchievementInit)]
    public class ServerAchievementInit : IWritable
    {
        public List<Achievement> Achievements { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Achievements.Count);
            Achievements.ForEach(a => a.Write(writer));
        }
    }
}
