using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellAbilityCharges)]
    public class ServerSpellAbilityCharges : IWritable
    {
        public uint SpellId { get; set; }
        public uint AbilityChargeCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellId, 18u);
            writer.Write(AbilityChargeCount);
        }
    }
}

