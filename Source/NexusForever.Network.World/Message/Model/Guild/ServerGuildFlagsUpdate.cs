using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildFlagsUpdate)]
    public class ServerGuildFlagsUpdate : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public GuildFlag Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Flags, 32u);
        }
    }
}
