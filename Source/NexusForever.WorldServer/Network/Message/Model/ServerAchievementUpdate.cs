using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAchievementUpdate)]
    public class ServerAchievementUpdate : IWritable
    {
        public bool Deleted { get; set; }
        public List<Achievement> Achievements { get; set; } = new List<Achievement>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Deleted);
            writer.Write(Achievements.Count);
            Achievements.ForEach(a => a.Write(writer));
        }
    }
}
