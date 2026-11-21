using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountItemCooldownSet)]
    public class ServerAccountItemCooldownSet : IWritable
    {
        public AccountItemCooldown Cooldown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Cooldown.Write(writer);
        }
    }
}
