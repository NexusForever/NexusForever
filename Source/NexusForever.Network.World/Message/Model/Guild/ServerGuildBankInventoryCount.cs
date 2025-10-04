using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    /// <summary> Updates stack count of item in guild bank inventory. </summary>
    [Message(GameMessageOpcode.ServerGuildBankInventoryCount)]
    public class ServerGuildBankInventoryCount : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public ulong ItemGuid { get; set; }
        public uint Count { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(ItemGuid);
            writer.Write(Count);
        }
    }
}
