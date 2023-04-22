using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildMemberRemove)]
    public class ServerGuildMemberRemove : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public TargetPlayerIdentity PlayerIdentity { get; set; }
        public ushort Unknown0 { get; set; }
        public ushort Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            PlayerIdentity.Write(writer);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
        }
    }
}
