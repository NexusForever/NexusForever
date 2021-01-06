using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildMemberChange)]
    public class ServerGuildMemberChange : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public GuildMember GuildMember { get; set; }
        public ushort MemberCount { get; set; }
        public ushort OnlineMemberCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            GuildMember.Write(writer);
            writer.Write(MemberCount);
            writer.Write(OnlineMemberCount);
        }
    }
}
