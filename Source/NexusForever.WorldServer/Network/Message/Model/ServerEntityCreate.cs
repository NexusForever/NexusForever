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
    [Message(GameMessageOpcode.ServerEntityCreate, MessageDirection.Server)]
    public class ServerEntityCreate : IWritable
    {
        #region Unknown Structures

        public class UnknownStructure88 : IWritable
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
                        writer.Write(Unknown1, 17);
                        break;
                }
            }
        }

        public class UnknownStructureB0 : IWritable
        {
            public byte Type { get; set; }
            public bool Unknown0 { get; set; }
            public ulong Unknown1 { get; set; }
            public ushort Unknown2 { get; set; }

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
                        writer.Write(Unknown2, 14);
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
        public byte Unknown60 { get; set; }
        public IEntityModel EntityModel { get; set; }
        public List<StatValue> Stats { get; set; } = new List<StatValue>();
        public uint Unknown68 { get; set; }
        public Dictionary<EntityCommand, IEntityCommand> Commands { get; set; } = new Dictionary<EntityCommand, IEntityCommand>();
        public List<PropertyValue> Properties { get; set; } = new List<PropertyValue>();
        public List<ItemVisual> VisibleItems { get; set; } = new List<ItemVisual>();
        public List<UnknownStructure88> Unknown88 { get; } = new List<UnknownStructure88>();
        public uint Unknown8C { get; set; }
        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }
        public uint Unknown98 { get; set; }
        public ulong Unknown9C { get; set; }
        public UnknownStructureA8 UnknownA8 { get; set; } = new UnknownStructureA8();
        public UnknownStructureB0 UnknownB0 { get; set; } = new UnknownStructureB0();
        public UnknownStructureC8 UnknownC8 { get; set; } = new UnknownStructureC8();
        public ushort UnknownD4 { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort OutfitInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Type, 6);
            EntityModel.Write(writer);
            writer.Write(Unknown60);

            writer.Write((byte)Stats.Count, 5);
            Stats.ForEach(o => o.Write(writer));

            writer.Write(Unknown68);

            writer.Write((byte)Commands.Count, 5);
            foreach (KeyValuePair<EntityCommand, IEntityCommand> pair in Commands)
            {
                writer.Write(pair.Key, 5);
                pair.Value.Write(writer);
            }

            writer.Write((byte)Properties.Count);
            Properties.ForEach(o => o.Write(writer));

            writer.Write((byte)VisibleItems.Count, 7);
            VisibleItems.ForEach(o => o.Write(writer));

            writer.Write((short)Unknown88.Count, 9);
            Unknown88.ForEach(o => o.Write(writer));

            writer.Write(Unknown8C);
            writer.Write(Faction1, 14);
            writer.Write(Faction2, 14);
            writer.Write(Unknown98);
            writer.Write(Unknown9C);

            UnknownA8.Write(writer);
            UnknownB0.Write(writer);
            UnknownC8.Write(writer);

            writer.Write(UnknownD4, 14);
            writer.Write(DisplayInfo, 17);
            writer.Write(OutfitInfo, 15);
        }
    }
}
