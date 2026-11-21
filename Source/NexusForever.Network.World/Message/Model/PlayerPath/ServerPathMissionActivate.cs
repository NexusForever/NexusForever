using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathMissionActivate)]
    public class ServerPathMissionActivate : IWritable
    {
        public List<Mission> Missions { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Missions.Count);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
