using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // Must be sent same time or after SpellEffects are applied to the unit.
    // The SpellEffectUniqueId must match the id from the SpellCast result in ServerSpellGo/ServerSpellExecute
    // This triggers the restrictions and VisualEffects for the CC state.
    [Message(GameMessageOpcode.ServerEntityCCStateSet)]
    public class ServerEntityCCStateSet : IWritable
    {
        public uint UnitId { get; set; }
        public CCState CCType { get; set; }
        public uint SpellEffectUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(CCType, 5u);
            writer.Write(SpellEffectUniqueId);
        }
    }
}
