using NexusForever.Game.Static.Combat;

namespace NexusForever.Network.World.Combat
{
    public class CombatLogCCState : ICombatLog
    {
        public CombatLogType Type => CombatLogType.CCState;

        public byte State { get; set; } // 5u
        public bool BRemoved { get; set; }
        public uint InterruptArmorTaken { get; set; }
        public byte Result { get; set; } // 4u
        public ushort Unknown0 { get; set; } // 14u
        public CombatLogCastData CastData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(State, 5u);
            writer.Write(BRemoved);
            writer.Write(InterruptArmorTaken);
            writer.Write(Result, 4u);
            writer.Write(Unknown0, 14u);
            CastData.Write(writer);
        }
    }
}
