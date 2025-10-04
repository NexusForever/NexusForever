using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildBankInventoryAdd)]
    public class ServerGuildBankInventoryAdd : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public InventoryItem InventoryItem { get; set; } // Make sure ItemLocation is a GuildBankTab

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            InventoryItem.Write(writer);
        }
    }
}
