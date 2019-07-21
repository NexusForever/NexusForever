using System;
using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCreate)]
    public class ServerEntityCreate : IWritable
    {
        #region Unknown Structures

        public class SpellInit : IWritable
        {
            public void Write(GamePacketWriter writer)
            {
                throw new NotImplementedException();
            }
        }

        public class UnknownStructureA8 : IWritable
        {
            public byte Type { get; set; }
            public bool Unknown0 { get; set; }
            public uint Unknown1 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unknown0);
                        break;
                    case 1:
                        writer.Write(Unknown1, 17u);
                        break;
                }
            }
        }

        public class WorldPlacement : IWritable
        {
            public byte Type { get; set; }
            public bool Unknown0 { get; set; }
            public ulong ActivePropId { get; set; }
            public ushort SocketId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2u);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unknown0);
                        break;
                    case 1:
                        writer.Write(ActivePropId);
                        writer.Write(SocketId, 14u);
                        break;
                }
            }
        }

        public class UnknownStructureC8 : IWritable
        {
            public byte Type { get; set; }
            public bool Unknown0 { get; set; }
            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unknown0);
                        break;
                    case 1:
                        writer.Write(Unknown1);
                        writer.Write(Unknown2, 18);
                        break;
                }
            }
        }

        #endregion

        public uint Guid { get; set; }
        public EntityType Type { get; set; }
        public byte CreateFlags { get; set; }
        public IEntityModel EntityModel { get; set; }
        public List<StatValue> Stats { get; set; } = new List<StatValue>();
        public uint Time { get; set; }
        public List<(EntityCommand, IEntityCommandModel)> Commands { get; set; } = new List<(EntityCommand, IEntityCommandModel)>();
        public List<PropertyValue> Properties { get; set; } = new List<PropertyValue>();
        public List<ItemVisual> VisibleItems { get; set; } = new List<ItemVisual>();
        public List<SpellInit> SpellInitData { get; } = new List<SpellInit>();
        public uint CurrentSpellUniqueId { get; set; }
        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }
        public uint UnitTagOwner { get; set; }
        public ulong GroupTagOwner { get; set; }
        public UnknownStructureA8 UnknownA8 { get; set; } = new UnknownStructureA8();
        public WorldPlacement WorldPlacementData { get; set; } = new WorldPlacement();
        public UnknownStructureC8 UnknownC8 { get; set; } = new UnknownStructureC8();
        public ushort MiniMapMarker { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort OutfitInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Type, 6);
            EntityModel.Write(writer);
            writer.Write(CreateFlags);

            writer.Write((byte)Stats.Count, 5);
            Stats.ForEach(o => o.WriteInitial(writer));

            writer.Write(Time);

            writer.Write((byte)Commands.Count, 5);
            foreach ((EntityCommand command, IEntityCommandModel entityCommand) in Commands)
            {
                writer.Write(command, 5);
                entityCommand.Write(writer);
            }

            writer.Write((byte)Properties.Count);
            Properties.ForEach(o => o.Write(writer));

            writer.Write((byte)VisibleItems.Count, 7);
            VisibleItems.ForEach(o => o.Write(writer));

            writer.Write((short)SpellInitData.Count, 9);
            SpellInitData.ForEach(o => o.Write(writer));

            writer.Write(CurrentSpellUniqueId);
            writer.Write(Faction1, 14);
            writer.Write(Faction2, 14);
            writer.Write(UnitTagOwner);
            writer.Write(GroupTagOwner);

            UnknownA8.Write(writer);
            WorldPlacementData.Write(writer);
            UnknownC8.Write(writer);

            writer.Write(MiniMapMarker, 14);
            writer.Write(DisplayInfo, 17);
            writer.Write(OutfitInfo, 15);
        }
    }
}
