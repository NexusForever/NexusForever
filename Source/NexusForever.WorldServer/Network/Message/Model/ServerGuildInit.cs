using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInit)]
    public class ServerGuildInit : IWritable
    {
        public uint NameplateIndex { get; set; }
        public List<GuildData> Guilds { get; set; } = new List<GuildData>();
        public List<GuildMember> Self { get; set; } = new List<GuildMember>();
        public List<GuildPlayerLimits> SelfPrivate { get; set; } = new List<GuildPlayerLimits>();

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
