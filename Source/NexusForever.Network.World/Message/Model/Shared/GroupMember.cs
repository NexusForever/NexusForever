using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class GroupMember : IWritable
    {
        public class UnknownStruct0 : IWritable
        {
            public ushort Unknown6 { get; set; } = 0;
            public byte Unknown7 { get; set; } = 48;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown6);
                writer.Write(Unknown7);
            }
        }

        public class PrimeLevelInfo : IWritable
        {
            public ushort WorldId { get; set; }
            public ushort PrimeLevelAchieved { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(WorldId, 15u);
                writer.Write(PrimeLevelAchieved);
            }
        }

        public string Name { get; set; }
        public Faction Faction { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Sex Sex { get; set; }
        public byte Level { get; set; }
        public byte EffectiveLevel { get; set; }
        public Game.Static.Entity.Path Path { get; set; }
        public uint Unknown4 { get; set; }
        public ushort GroupMemberId { get; set; }

        public UnknownStruct0[] SomeStatList = new UnknownStruct0[5];
        public List<PrimeLevelInfo> PrimeLevels { get; set; } = new List<PrimeLevelInfo>();

        public Identity Mentee { get; set; }

        public uint Health { get; set; }
        public ushort HealthMax { get; set; }
        public ushort ShieldCapacity { get; set; }
        public ushort ShieldCapacityMax { get; set; }
        public ushort InterruptArmor { get; set; }
        public ushort InterruptArmorMax { get; set; }
        public ushort Absorption { get; set; }
        public ushort AbsorptionMax { get; set; }
        public ushort Focus { get; set; }
        public ushort BaseFocusPool { get; set; }
        public ushort HealingAbsorption { get; set; }
        public ushort HealingAbsorptionMax { get; set; }
        public ushort Unknown22 { get; set; }

        public ushort Realm { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint MapId { get; set; }
        public uint PhaseId { get; set; } = 1;
        public bool InInstance { get; set; }

        public uint PhasesCanBePerceived { get; set; }
        public uint PhasesCanPerceive { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Name);
            writer.Write(Faction, 14u);
            writer.Write(Race, 14u);
            writer.Write(Class, 14u);
            writer.Write(Sex, 2u);
            writer.Write(Level, 7u);
            writer.Write(EffectiveLevel, 7u);
            writer.Write(Path, 3u);
            writer.Write(Unknown4, 17u);
            writer.Write(GroupMemberId);

            for (var i = 0; i < 5; ++i)
            {
                SomeStatList[i] = new UnknownStruct0();
                SomeStatList[i].Write(writer);
            }

            if (Mentee == null)
            {
                writer.Write((ushort)0, 14u);
                writer.Write((ulong)0);
            }
            else
                Mentee.Write(writer);

            writer.Write(Health);
            writer.Write(HealthMax);
            writer.Write(ShieldCapacity);
            writer.Write(ShieldCapacityMax);
            writer.Write(InterruptArmor);
            writer.Write(InterruptArmorMax);
            writer.Write(Absorption);
            writer.Write(AbsorptionMax);
            writer.Write(Focus);
            writer.Write(BaseFocusPool);
            writer.Write(HealingAbsorption);
            writer.Write(HealingAbsorptionMax);
            writer.Write(Unknown22);
            writer.Write(Realm, 14u);
            writer.Write(WorldZoneId, 15u);
            writer.Write(MapId);
            writer.Write(PhaseId);
            writer.Write(InInstance);
            writer.Write(PhasesCanBePerceived);
            writer.Write(PhasesCanPerceive);

            writer.Write(PrimeLevels.Count);
            PrimeLevels.ForEach(i => i.Write(writer));
        }
    }
}
