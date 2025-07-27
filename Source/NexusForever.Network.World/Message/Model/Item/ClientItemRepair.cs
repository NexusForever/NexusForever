using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemRepair)]
    public class ClientItemRepair : IReadable
    {
        public ulong ItemGuid { get; private set; } // 0 if requesting RepairAll
        public ulong GuildId { get; private set; }
        public ulong Credits { get; private set; }

        // SINGLE ITEM REPAIR
        // ItemGuid != 0 when requesting a repair for a single item.
        // If GuildId != 0, Credits field will be 0 and means that the repair request should use whatever guild credits available first then use the player's credits for the balance.
        // If GuildId == 0, Credits field will be 0 however the message is only sent if the Client knows the player has enough credits for the repair. Server should still check the 
        // player has enough credits for the repair.

        // REPAIR ALL
        // ItemGuid == 0 when requesting a repair for all items in the player's inventory.
        // If GuildId != 0, Credits will be the amount of guild credits available the client knows it has available. Spend these first then use the player's credits for the balance.
        // If GuidlId == 0, Credits field will be the total amount of credits the Client knows the player has. Server should still check the player has enough credits for the repair.

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            GuildId = reader.ReadULong();
            Credits = reader.ReadULong();
        }
    }
}
