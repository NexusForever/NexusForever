using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterList)]
    public class ServerCharacterList : IWritable
    {
        public class Character : IWritable
        {
            // probably, x, y, z, yaw, pitch
            public class UnknownStructure2 : IWritable
            {
                public uint Unknown0 { get; set; }
                public uint Unknown4 { get; set; }
                public uint Unknown8 { get; set; }
                public uint UnknownC { get; set; }
                public uint Unknown10 { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Unknown0);
                    writer.Write(Unknown4);
                    writer.Write(Unknown8);
                    writer.Write(UnknownC);
                    writer.Write(Unknown10);
                }
            }

            public ulong Id { get; set; }
            public string Name { get; set; }
            public Sex Sex { get; set; }
            public Race Race { get; set; }
            public Class Class { get; set; }
            public uint Faction { get; set; }
            public uint Level { get; set; }
            public List<ItemVisual> Appearance { get; } = new List<ItemVisual>();
            public List<ItemVisual> Gear { get; } = new List<ItemVisual>();
            public ushort WorldId { get; set; }
            public ushort WorldZoneId { get; set; }
            public ushort RealmId { get; set; }
            public UnknownStructure2 Unknown3C { get; } = new UnknownStructure2();
            public byte Path { get; set; }
            public bool IsLocked { get; set; }
            public bool Unknown58 { get; set; }
            public uint GearMask { get; set; }
            public List<uint> Labels { get; } = new List<uint>();
            public List<uint> Values { get; } = new List<uint>();
            public List<float> Bones { get; } = new List<float>();
            public uint Unknown74 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id);
                writer.WriteStringWide(Name);
                writer.Write(Sex, 2);
                writer.Write(Race, 5);
                writer.Write(Class, 5);
                writer.Write(Faction);
                writer.Write(Level);

                writer.Write(Appearance.Count);
                foreach (ItemVisual item in Appearance)
                    item.Write(writer);

                writer.Write(Gear.Count);
                foreach (ItemVisual item in Gear)
                    item.Write(writer);

                writer.Write(WorldId, 15);
                writer.Write(WorldZoneId, 15);
                writer.Write(RealmId, 14);

                Unknown3C.Write(writer);

                writer.Write(Path, 3);
                writer.Write(IsLocked);
                writer.Write(Unknown58);
                writer.Write(GearMask);

                writer.Write(Labels.Count, 4);
                for (int i = 0; i < Labels.Count; i++)
                    writer.Write(Labels[i]);
                for (int i = 0; i < Labels.Count; i++)
                    writer.Write(Values[i]);

                writer.Write(Bones.Count);
                foreach (float value in Bones)
                    writer.Write(value);

                writer.Write(Unknown74);
            }
        }

        public ulong Id { get; set; }
        public List<Character> Characters { get; } = new List<Character>();
        public List<uint> Unknown14 { get; } = new List<uint>();
        public List<uint> Unknown1C { get; } = new List<uint>();
        public ushort RealmId { get; set; }
        public ushort Unknown28 { get; set; }
        public ulong Unknown30 { get; set; }
        public uint Unknown38 { get; set; }
        public uint Unknown3C { get; set; }
        public uint Unknown40 { get; set; }
        public uint Unknown44 { get; set; }
        public ushort Unknown48 { get; set; }
        public bool Unknown4C { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Id);
            writer.Write(Characters.Count);

            foreach (Character character in Characters)
                character.Write(writer);

            writer.Write(Unknown14.Count);
            foreach (uint value in Unknown14)
                writer.Write(value);

            writer.Write(Unknown1C.Count);
            foreach (uint value in Unknown1C)
                writer.Write(value);

            writer.Write(RealmId, 14);

            writer.Write(Unknown28, 14);
            writer.Write(Unknown30);

            writer.Write(Unknown38);
            writer.Write(Unknown3C);
            writer.Write(Unknown40);
            writer.Write(Unknown44);
            writer.Write(Unknown48, 14);
            writer.Write(Unknown4C);
        }
    }
}
