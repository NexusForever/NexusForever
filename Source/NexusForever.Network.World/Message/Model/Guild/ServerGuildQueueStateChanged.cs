using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Fires whenever the arena team or warparty enters a queue, leaves a queue, enters a match, or leaves a match.
    [Message(GameMessageOpcode.ServerGuildQueueStateChanged)]
    public class ServerGuildQueueStateChanged : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public GuildQueueState State { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(State, 3u);
        }
    }
}
