using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
