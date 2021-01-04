using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildRemove)]
    public class ServerGuildRemove : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
        }
    }
}
