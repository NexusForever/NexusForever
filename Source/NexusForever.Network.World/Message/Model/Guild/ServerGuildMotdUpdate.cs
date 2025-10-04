using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildMotdUpdate)]
    public class ServerGuildMotdUpdate : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public string MessageOfTheDay { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.WriteStringWide(MessageOfTheDay);
        }
    }
}
