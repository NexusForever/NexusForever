using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildRemove)]
    public class ServerGuildRemove : IWritable
    {
        public Identity GuildIdentity { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
        }
    }
}
