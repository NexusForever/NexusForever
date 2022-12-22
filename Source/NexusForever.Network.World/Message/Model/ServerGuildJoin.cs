using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildJoin)]
    public class ServerGuildJoin : IWritable
    {
        public GuildData GuildData { get; set; } = new();
        public GuildMember Self { get; set; } = new();
        public GuildPlayerLimits SelfPrivate { get; set; } = new();
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
