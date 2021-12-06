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

        public class UnknownStruct1 : IWritable
        {
            public ushort Unknown30 { get; set; }
            public ushort Unknown31 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown30, 15u);
                writer.Write(Unknown31);
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
        public List<UnknownStruct1> UnknownStruct1List { get; set; } = new List<UnknownStruct1>();

        public TargetPlayerIdentity MentoringTarget { get; set; }

        public uint Unknown10 { get; set; }
        public ushort Unknown11 { get; set; }
        public ushort Unknown12 { get; set; }
        public ushort Unknown13 { get; set; }
        public ushort Unknown14 { get; set; }
        public ushort Unknown15 { get; set; }
        public ushort Unknown16 { get; set; }
        public ushort Unknown17 { get; set; }
        public ushort Unknown18 { get; set; }
        public ushort Unknown19 { get; set; }
        public ushort Unknown20 { get; set; }
        public ushort Unknown21 { get; set; }
        public ushort Unknown22 { get; set; }

        public ushort Realm { get; set; }
        public ushort WorldZoneId { get; set; }
        public uint MapId { get; set; }
        public uint PhaseId { get; set; } = 1;
        public bool SyncedToGroup { get; set; }

        public uint Unknown28 { get; set; }
        public uint Unknown29 { get; set; }

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

            if (MentoringTarget == null)
            {
                writer.Write((ushort)0, 14u);
                writer.Write((ulong)0);
            }
            else
                MentoringTarget.Write(writer);

            writer.Write(Unknown10);
            writer.Write(Unknown11);
            writer.Write(Unknown12);
            writer.Write(Unknown13);
            writer.Write(Unknown14);
            writer.Write(Unknown15);
            writer.Write(Unknown16);
            writer.Write(Unknown17);
            writer.Write(Unknown18);
            writer.Write(Unknown19);
            writer.Write(Unknown20);
            writer.Write(Unknown21);
            writer.Write(Unknown22);
            writer.Write(Realm, 14u);
            writer.Write(WorldZoneId, 15u);
            writer.Write(MapId);
            writer.Write(PhaseId);
            writer.Write(SyncedToGroup);
            writer.Write(Unknown28);
            writer.Write(Unknown29);

            writer.Write(UnknownStruct1List.Count);
            UnknownStruct1List.ForEach(i => i.Write(writer));
        }
    }
}
