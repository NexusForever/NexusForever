using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    // Similar to ServerSpellThresholdClear but specifically for PressHold and RapidTap cast method spells.
    // Explicitly does not affect ChargeRelease spells
    [Message(GameMessageOpcode.ServerSpellThresholdCancel)]
    public class ServerSpellThresholdCancel : IWritable
    {
        public uint SpellId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellId, 18u);
        }
    }
}

