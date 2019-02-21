using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Entity
{
    public abstract class WorldEntity : GridEntity
    {
        public EntityType Type { get; }
        public EntityCreateFlag CreateFlags { get; set; }
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Dictionary<Stat, StatValue> Stats { get; private set; } = new Dictionary<Stat, StatValue>();
        public Dictionary<Property, PropertyValue> Properties { get; } = new Dictionary<Property, PropertyValue>();

        public uint DisplayInfo { get; protected set; }
        public ushort OutfitInfo { get; protected set; }
        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }

        /// <summary>
        /// Guid of the <see cref="WorldEntity"/> currently targeted.
        /// </summary>
        public uint TargetGuid { get; set; }

        /// <summary>
        /// Guid of the <see cref="Player"/> currently controlling this <see cref="WorldEntity"/>.
        /// </summary>
        public uint ControllerGuid { get; set; }

        private readonly Dictionary<ItemSlot, ItemVisual> itemVisuals = new Dictionary<ItemSlot, ItemVisual>();

        /// <summary>
        /// Create a new <see cref="WorldEntity"/> with supplied <see cref="EntityType"/>.
        /// </summary>
        protected WorldEntity(EntityType type)
        {
            Type = type;
        }

        protected abstract IEntityModel BuildEntityModel();

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occured.
        /// </summary>
        public override void Update(double lastTick)
        {
            // deliberately empty
        }

        public virtual ServerEntityCreate BuildCreatePacket()
        {
            return new ServerEntityCreate
            {
                Guid        = Guid,
                Type        = Type,
                EntityModel = BuildEntityModel(),
                CreateFlags   = (byte)CreateFlags,
                Stats       = Stats.Values.ToList(),

                Commands =
                {
                    {
                        EntityCommand.SetPosition,
                        new SetPositionCommand
                        {
                            Position = new Position(Position)
                        }
                    },
                    {
                        EntityCommand.SetRotation,
                        new SetRotationCommand
                        {
                            Position = new Position(Rotation)
                        }
                    }
                },
                VisibleItems = itemVisuals.Values.ToList(),
                Properties   = Properties.Values.ToList(),
                Faction1     = Faction1,
                Faction2     = Faction2,
                DisplayInfo  = DisplayInfo,
                OutfitInfo   = OutfitInfo
            };
        }


        protected void SetProperty(Property property, float value, float baseValue = 0.0f)
        {
            if (Properties.ContainsKey(property))
                Properties[property].Value = value;
            else
                Properties.Add(property, new PropertyValue(property, baseValue, value));
        }

        protected float? GetPropertyValue(Property property)
        {
            return Properties.ContainsKey(property) ? Properties[property].Value : default;
        }

        public dynamic GetStatValue(Stat stat)
        {
            if (Stats.TryGetValue(stat, out StatValue value))
                return StatValue.GetStatType(stat) == StatValue.StatType.Float ? (float)value.Value : (uint)value.Value;
            else
                return StatValue.GetStatType(stat) == StatValue.StatType.Float ? 0f : 0u;
        }

        protected void SetStat(Stat stat, float value)
        {
            if (Stats.TryGetValue(stat, out StatValue statValue))
            {
                if (statValue.Value != value)
                {
                    statValue.SaveMask |= StatSaveMask.Modified;
                    statValue.Value = value;
                }
            }
            else
            {
                Stats.Add(stat, new StatValue(stat, value));
                Stats[stat].SaveMask |= StatSaveMask.Create;
            }

            if (Stats[stat].SaveMask != StatSaveMask.None)
                OnStatUpdate(stat, value);
        }

        protected void SetStat(Stat stat, uint value)
        {
            SetStat(stat, (float)value);
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> for multiple supplied <see cref="ItemSlot"/>.
        /// </summary>
        public void SetAppearance(IEnumerable<ItemVisual> visuals)
        {
            foreach (ItemVisual visual in visuals)
                SetAppearance(visual);
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> for supplied <see cref="ItemVisual"/>.
        /// </summary>
        public void SetAppearance(ItemVisual visual)
        {
            if (visual.DisplayId != 0)
            {
                if (!itemVisuals.ContainsKey(visual.Slot))
                    itemVisuals.Add(visual.Slot, visual);
                else
                    itemVisuals[visual.Slot] = visual;
            }
            else
                itemVisuals.Remove(visual.Slot);
        }

        public IEnumerable<ItemVisual> GetAppearance()
        {
            return itemVisuals.Values;
        }

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="Player"/>'s in range.
        /// </summary>
        public void EnqueueToVisible(IWritable message, bool includeSelf = false)
        {
            foreach (WorldEntity entity in visibleEntities.Values)
            {
                var player = entity as Player;
                if (player == null)
                    continue;

                if (!includeSelf && (Guid == entity.Guid || ControllerGuid == entity.Guid))
                    continue;

                player.Session.EnqueueMessageEncrypted(message);
            }
        }

        public void OnStatUpdate(Stat stat, float value)
        {
            if (StatValue.SendUpdate(stat))
            {
                if (StatValue.GetStatType(stat) == StatValue.StatType.Float)
                {
                    EnqueueToVisible(new ServerEntityStatUpdateFloat
                    {
                        UnitId = Guid,
                        Stat = new StatValue(stat, value)
                    }, this is Player);
                }
                else if (StatValue.GetStatType(stat) == StatValue.StatType.Int)
                {
                    EnqueueToVisible(new ServerEntityStatUpdateInt
                    {
                        UnitId = Guid,
                        Stat = new StatValue(stat, value)
                    }, this is Player);
                }
            }
            else if(stat == Stat.Health)
            {
                EnqueueToVisible(new ServerUpdateHealth
                {
                    UnitId = Guid,
                    Health = (uint)value,
                    UnknownMask = 32768
                }, this is Player);
            }

            switch(stat)
            {
                case Stat.Level:
                    if (this is Player player)
                        player.LevelUp();
                    break;

                case Stat.Health:
                    // check if it's dead, Jim
                    break;
            }
        }
    }
}
