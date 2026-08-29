using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildRename)]
    public class ServerGuildRename : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public string Name { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.WriteStringWide(Name);
        }
    }
}
