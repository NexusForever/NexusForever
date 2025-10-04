using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildInfoMessageUpdate)]
    public class ServerGuildInfoMessageUpdate : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public string InfoMessage { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.WriteStringWide(InfoMessage);
        }
    }
}
