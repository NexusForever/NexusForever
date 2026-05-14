using NexusForever.Game.Static.Combat;
using NexusForever.Game.Static.Combat.CrowdControl;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogCCState : ICombatLog
    {
        public CombatLogType Type => CombatLogType.CCState;

        public CCState State { get; set; } // 5u
        public bool BRemoved { get; set; }
        public uint InterruptArmorTaken { get; set; }
        public CCStateApplyRulesResult Result { get; set; } // 4u
        public ushort CcStateDiminishingReturnsId { get; set; } // 14u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(State, 5u);
            writer.Write(BRemoved);
            writer.Write(InterruptArmorTaken);
            writer.Write(Result, 4u);
            writer.Write(CcStateDiminishingReturnsId, 14u);
            CastData.Write(writer);
        }
    }
}
