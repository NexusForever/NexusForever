using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerRemoveCCState)]
    public class ServerRemoveCCState : IWritable
    {
        public uint UnitId { get; set; }
        public CCStateType CCType { get; set; }
        public uint SpellCastUniqueId { get; set; } // Must match the SpellCastUniqueId from ServerSpellGo/ServerSpellExecute
        public uint SpellEffectUniqueId { get; set; } // Must match the SpellEffectUniqueId from ServerSpellGo/ServerSpellExecute. TBC can be 0 for some CCs
        public bool Unknown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(CCType, 5u);
            writer.Write(SpellCastUniqueId);
            writer.Write(SpellEffectUniqueId);
            writer.Write(Unknown);
        }
    }
}
