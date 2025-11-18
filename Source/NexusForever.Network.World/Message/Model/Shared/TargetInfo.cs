using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TargetInfo : IWritable // same used for 0x0818
    {
        public class EffectInfo : IWritable
        {
            public uint Spell4EffectId { get; set; }
            public uint EffectUniqueId { get; set; }
            public uint DelayTime { get; set; }
            public int TimeRemaining { get; set; }
            public byte InfoType { get; set; }

            public SpellEffectResult EffectResultData { get; set; } = new();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Spell4EffectId, 19u);
                writer.Write(EffectUniqueId);
                writer.Write(DelayTime);
                writer.Write(TimeRemaining);
                writer.Write(InfoType, 2u);

                if (InfoType == 1)
                    EffectResultData.Write(writer);
                else
                    writer.Write(0u, 1u);
            }

        }

        public uint UnitId { get; set; }
        public byte Ndx { get; set; }
        public byte TargetFlags { get; set; }
        public ushort InstanceCount { get; set; }
        public CombatResult CombatResult { get; set; }

        public List<EffectInfo> EffectInfoData { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Ndx);
            writer.Write(TargetFlags);
            writer.Write(InstanceCount);
            writer.Write(CombatResult, 4u);

            writer.Write(EffectInfoData.Count, 8u);
            EffectInfoData.ForEach(u => u.Write(writer));
        }

    }
}
