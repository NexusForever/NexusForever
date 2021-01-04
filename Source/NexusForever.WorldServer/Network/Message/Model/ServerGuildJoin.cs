using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildJoin)]
    public class ServerGuildJoin : IWritable
    {
        public GuildData GuildData { get; set; } = new GuildData();
        public GuildMember Self { get; set; } = new GuildMember();
        public GuildPlayerLimits SelfPrivate { get; set; } = new GuildPlayerLimits();
        public bool Nameplate { get; set; } = false;

        public void Write(GamePacketWriter writer)
        {
            GuildData.Write(writer);
            Self.Write(writer);
            SelfPrivate.Write(writer);
            writer.Write(Nameplate);
        }
    }
}
