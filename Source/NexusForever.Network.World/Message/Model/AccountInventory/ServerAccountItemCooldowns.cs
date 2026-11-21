using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountItemCooldowns)]
    public class ServerAccountItemCooldowns : IWritable
    {
        public List<AccountItemCooldown> AccountItemCooldowns { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItemCooldowns.Count);
            AccountItemCooldowns.ForEach(cooldown => cooldown.Write(writer));
        }
    }
}
