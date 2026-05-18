using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildBankInventoryRemove)]
    public class ServerGuildBankInventoryRemove : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public ulong ItemGuid { get; private set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(ItemGuid);
        }
    }
}
