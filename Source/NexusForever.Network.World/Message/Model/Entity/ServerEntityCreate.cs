using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCreate)]
    public class ServerEntityCreate : IWritable
    {
        public class SpellInit : IWritable
        {
            public void Write(GamePacketWriter writer)
            {
                // Implemented in spell PR https://github.com/NexusForever/NexusForever/pull/489
                throw new NotImplementedException();
            }
        }

        public class VendorInfo : IWritable
        {
            public byte Type { get; set; } // 1 = isVendor
            public bool Unused { get; set; }
            public uint VendorInteractionPrerequisiteId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unused);
                        break;
                    case 1:
                        writer.Write(VendorInteractionPrerequisiteId, 17u);
                        break;
                }
            }
        }

        public class WorldPlacement : IWritable
        {
            public byte Type { get; set; } // 1 = isWorldProp
            public bool Unused { get; set; }
            public ulong ActivePropId { get; set; }
            public ushort WorldSocketId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2u);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unused);
                        break;
                    case 1:
                        writer.Write(ActivePropId);
                        writer.Write(WorldSocketId, 14u);
                        break;
                }
            }
        }

        public class TargetClusterInfo : IWritable
        {
            public byte Type { get; set; } // 1 = isInCluster
            public bool Unused { get; set; }
            public uint TargetClusterId { get; set; }
            public uint Unknown2 { get; set; } // non-zero in sniffs, but likely unused by client

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Type, 2);

                switch (Type)
                {
                    case 0:
                        writer.Write(Unused);
                        break;
                    case 1:
                        writer.Write(TargetClusterId);
                        writer.Write(Unknown2, 18);
                        break;
                }
            }
        }

        public uint UnitId { get; set; }
        public EntityType Type { get; set; }
        public IEntityModel EntityModel { get; set; }
        public EntityCreateFlag CreateFlags { get; set; }
        public List<StatValueInitial> Stats { get; set; } = [];
        public uint CommandTime { get; set; }
        public List<INetworkEntityCommand> Commands { get; set; } = [];
        public List<PropertyValue> Properties { get; set; } = [];
        public List<ItemVisual> VisibleItems { get; set; } = [];
        public List<SpellInit> SpellInitData { get; } = [];
        public uint CurrentSpellCastUniqueId { get; set; }
        public Faction MutableFactionId { get; set; } // Can be updated after creation with ServerEntityFaction. Send the same value as BaseFactionId if no reason to override it
        public Faction BaseFactionId { get; set; } // Not changeable after creation
        public uint TagOwnerUnitId { get; set; }
        public ulong TagOwnerGroupId { get; set; }
        public VendorInfo VendorData { get; set; } = new();
        public WorldPlacement WorldPlacementData { get; set; } = new();
        public TargetClusterInfo TargetClusterData { get; set; } = new();
        public ushort MiniMapMarker { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort OutfitInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Type, 6);
            EntityModel.Write(writer);
            writer.Write(CreateFlags, 8);

            writer.Write((byte)Stats.Count, 5);
            Stats.ForEach(o => o.Write(writer));

            writer.Write(CommandTime);

            writer.Write((byte)Commands.Count, 5);
            foreach (INetworkEntityCommand command in Commands)
            {
                writer.Write(command.Command, 5);
                command.Model.Write(writer);
            }

            writer.Write((byte)Properties.Count);
            Properties.ForEach(o => o.Write(writer));

            writer.Write((byte)VisibleItems.Count, 7);
            VisibleItems.ForEach(o => o.Write(writer));

            writer.Write((short)SpellInitData.Count, 9);
            SpellInitData.ForEach(o => o.Write(writer));

            writer.Write(CurrentSpellCastUniqueId);
            writer.Write(MutableFactionId, 14);
            writer.Write(BaseFactionId, 14);
            writer.Write(TagOwnerUnitId);
            writer.Write(TagOwnerGroupId);

            VendorData.Write(writer);
            WorldPlacementData.Write(writer);
            TargetClusterData.Write(writer);

            writer.Write(MiniMapMarker, 14);
            writer.Write(DisplayInfo, 17);
            writer.Write(OutfitInfo, 15);
        }
    }
}
