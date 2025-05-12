using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathEpisodeProgress)]
    public class ServerPathEpisodeProgress : IWritable
    {
        public class Mission : IWritable
        {
            public ushort PathMissionId { get; set; }
            public bool Completed { get; set; }
            public uint ObjectiveCompletionFlags { get; set; }
            public uint StateFlags { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PathMissionId, 15);
                writer.Write(Completed);
                writer.Write(ObjectiveCompletionFlags);
                writer.Write(StateFlags);
            }
        }

        public ushort EpisodeId { get; set; }
        public List<Mission> Missions { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EpisodeId, 14);
            writer.Write(Missions.Count, 16);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
