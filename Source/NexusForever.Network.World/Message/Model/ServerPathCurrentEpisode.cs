using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSetCurrentEpisode)]
    public class ServerPathSetCurrentEpisode : IWritable
    {
        public ushort Unknown0 { get; set; } // Not used in the client
        public ushort PathEpisodeId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 15);
            writer.Write(PathEpisodeId, 14);
        }
    }
}
