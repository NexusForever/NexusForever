using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerEntityCCStateRemove)]
    public class ServerEntityCCStateRemove : IWritable
    {
        public uint UnitId { get; set; }
        public CCState CCType { get; set; }
        public uint SpellCastUniqueId { get; set; } // Must match the SpellCastUniqueId from ServerSpellGo/ServerSpellExecute
        public uint SpellEffectUniqueId { get; set; } // Must match the SpellEffectUniqueId from ServerSpellGo/ServerSpellExecute. TBC can be 0 for some CCs
        public bool Removed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(CCType, 5u);
            writer.Write(SpellCastUniqueId);
            writer.Write(SpellEffectUniqueId);
            writer.Write(Removed);
        }
    }
}
