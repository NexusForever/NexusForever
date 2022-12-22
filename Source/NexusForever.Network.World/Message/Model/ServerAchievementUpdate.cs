using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAchievementUpdate)]
    public class ServerAchievementUpdate : IWritable
    {
        public bool Deleted { get; set; }
        public List<Achievement> Achievements { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Deleted);
            writer.Write(Achievements.Count);
            Achievements.ForEach(a => a.Write(writer));
        }
    }
}
