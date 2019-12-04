using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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

