using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildRoster)]
    public class ServerGuildRoster : IWritable
    {
        public ushort GuildRealm { get; set; }
        public ulong GuildId { get; set; }
        public List<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();
        public bool Done { get; set; } = true;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildRealm, 14u);
            writer.Write(GuildId);
            writer.Write(GuildMembers.Count);
            GuildMembers.ForEach(w => w.Write(writer));
            writer.Write(Done);
        }
    }
}
