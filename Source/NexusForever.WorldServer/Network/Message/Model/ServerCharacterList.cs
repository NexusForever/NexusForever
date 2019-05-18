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
            public class Location : IWritable
            {
                public Position Position { get; set; }
                public float Yaw { get; set; }
                public float Pitch { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    Position.Write(writer);
                    writer.Write(Yaw);
                    writer.Write(Pitch);
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
            public Location LocationData { get; set; } = new Location();
            public byte Path { get; set; }
            public bool Disabled { get; set; }
            public bool RequiresRename { get; set; }
            public uint GearMask { get; set; }
            public List<uint> Labels { get; } = new List<uint>();
            public List<uint> Values { get; } = new List<uint>();
            public List<float> Bones { get; } = new List<float>();
            public float LastLoggedOutDays { get; set; }

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

                LocationData.Write(writer);

                writer.Write(Path, 3);
                writer.Write(Disabled);
                writer.Write(RequiresRename);
                writer.Write(GearMask);

                writer.Write(Labels.Count, 4);
                for (int i = 0; i < Labels.Count; i++)
                    writer.Write(Labels[i]);
                for (int i = 0; i < Labels.Count; i++)
                    writer.Write(Values[i]);

                writer.Write(Bones.Count);
                foreach (float value in Bones)
                    writer.Write(value);

                writer.Write(LastLoggedOutDays);
            }
        }

        public ulong ServerTime { get; set; }
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
        public bool FreeLevel50 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerTime);
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
            writer.Write(FreeLevel50);
        }
    }
}
