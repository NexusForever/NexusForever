using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathEpisodeProgress)]
    public class ServerPathEpisodeProgress : IWritable
    {
        public ushort EpisodeId { get; set; }
        public List<Mission> Missions { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EpisodeId, 14);
            writer.Write(Missions.Count, 16);
            Missions.ForEach(mission => {
                writer.Write(mission.PathMissionId, 15u);
                writer.Write(mission.Completed);
                writer.Write(mission.ObjectiveCompletionFlags);
                writer.Write(mission.StateFlags);
                });
        }
    }
}
