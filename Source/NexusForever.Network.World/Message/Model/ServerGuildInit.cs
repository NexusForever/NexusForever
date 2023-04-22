using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInit)]
    public class ServerGuildInit : IWritable
    {
        public uint NameplateIndex { get; set; }
        public List<GuildData> Guilds { get; set; } = new();
        public List<GuildMember> Self { get; set; } = new();
        public List<GuildPlayerLimits> SelfPrivate { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guilds.Count);
            writer.Write(NameplateIndex);
            Guilds.ForEach(w => w.Write(writer));
            Self.ForEach(w => w.Write(writer));
            SelfPrivate.ForEach(w => w.Write(writer));
        }
    }
}
