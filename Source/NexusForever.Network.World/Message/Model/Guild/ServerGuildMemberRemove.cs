using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildMemberRemove)]
    public class ServerGuildMemberRemove : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public Identity MemberToRemove { get; set; }
        public ushort MemberCount { get; set; }
        public ushort OnlineMemberCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            MemberToRemove.Write(writer);
            writer.Write(MemberCount);
            writer.Write(OnlineMemberCount);
        }
    }
}
