using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathCurrentEpisode)]
    public class ServerPathCurrentEpisode : IWritable
    {
        public ushort Unknown0 { get; set; }
        public ushort EpisodeId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 15);
            writer.Write(EpisodeId, 14);
        }
    }
}
