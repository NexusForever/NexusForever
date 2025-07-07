using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterList)]
    public class ServerCharacterList : IWritable
    {
        public class Character : IWritable
        {
            // probably, x, y, z, yaw, pitch
            public class WorldLocation : IWritable
            {
                public Position Position { get; set; } = new Position();
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
            public List<ItemVisual> Appearance { get; } = [];
            public List<ItemVisual> Gear { get; } = [];
            public ushort WorldId { get; set; }
            public ushort WorldZoneId { get; set; }
            public ushort RealmId { get; set; }
            public WorldLocation Location { get; } = new();
            public byte Path { get; set; }
            public bool IsLocked { get; set; }
            public bool RequiresRename { get; set; }
            public uint GearMask { get; set; }
            public List<uint> Labels { get; } = [];
            public List<uint> Values { get; } = [];
            public List<float> Bones { get; } = [];
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

                Location.Write(writer);

                writer.Write(Path, 3);
                writer.Write(IsLocked);
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
        public List<Character> Characters { get; } = [];
        public List<uint> EnabledCharacterCreationIds { get; set; } = []; // Unclear if sending these has any effect.
        public List<uint> DisabledCharacterCreationIds { get; set; } = [];
        public ushort RealmId { get; set; }
        public Identity CharacterRemoveIdentity { get; set; } = new Identity();
        public uint CharacterRemoveTime { get; set; }
        public uint CharacterReservationCount { get; set; }
        public uint MaxNumberCharacters { get; set; }
        public uint AdditionalCount { get; set; }
        public Faction FactionRestriction { get; set; }
        public bool FreeLevel50 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerTime);
            writer.Write(Characters.Count);

            foreach (Character character in Characters)
                character.Write(writer);

            writer.Write(EnabledCharacterCreationIds.Count);
            foreach (uint value in EnabledCharacterCreationIds)
                writer.Write(value);

            writer.Write(DisabledCharacterCreationIds.Count);
            foreach (uint value in DisabledCharacterCreationIds)
                writer.Write(value);

            writer.Write(RealmId, 14);

            CharacterRemoveIdentity.Write(writer);

            writer.Write(CharacterRemoveTime);
            writer.Write(CharacterReservationCount);
            writer.Write(MaxNumberCharacters); 
            writer.Write(AdditionalCount);
            writer.Write(FactionRestriction, 14);
            writer.Write(FreeLevel50);
        }
    }
}
