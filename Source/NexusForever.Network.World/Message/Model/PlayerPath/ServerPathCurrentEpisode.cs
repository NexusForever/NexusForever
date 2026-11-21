using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSetCurrentEpisode)]
    public class ServerPathSetCurrentEpisode : IWritable
    {
        public ushort Unused { get; set; } // shows values in sniffs but client does not use
        public ushort PathEpisodeId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unused, 15);
            writer.Write(PathEpisodeId, 14);
        }
    }
}
