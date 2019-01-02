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
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Dictionary<Stat, StatValue> Stats { get; } = new Dictionary<Stat, StatValue>();
        public Dictionary<Property, PropertyValue> Properties { get; } = new Dictionary<Property, PropertyValue>();

        public uint DisplayInfo { get; protected set; }
        public ushort OutfitInfo { get; protected set; }
        public Faction Faction1 { get; protected set; }
        public Faction Faction2 { get; protected set; }

        public bool IsLoading { get; protected set; }

        protected Dictionary<ItemSlot, ItemVisual> itemVisuals = new Dictionary<ItemSlot, ItemVisual>();

        protected WorldEntity(EntityType type)
        {
            Type = type;
        }

        protected abstract IEntityModel BuildEntityModel();

        public override void Update(double lastTick)
        {

        }

        public virtual ServerEntityCreate BuildCreatePacket()
        {
            return new ServerEntityCreate
            {
                Guid      = Guid,
                Type      = Type,
                EntityModel    = BuildEntityModel(),
                Unknown60 = 1,
                Stats     = Stats.Values.ToList(),
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
                Faction1    = Faction1,
                Faction2    = Faction2,
                DisplayInfo = DisplayInfo,
                OutfitInfo = OutfitInfo
            };
        }

        protected void SetProperty(Property property, float value, float baseValue)
        {
            if (Properties.ContainsKey(property))
            {
                if (Properties[property].Value != value || Properties[property].BaseValue != baseValue)
                    Properties[property].IsModified = true;
                Properties[property].Value = value;
                Properties[property].BaseValue = baseValue;
            }
            else
                Properties.Add(property, new PropertyValue(property, baseValue, value));
        }

        protected void SetPropertyValue(Property property, float value)
        {
            if (Properties.ContainsKey(property))
            {
                if (Properties[property].Value != value)
                    Properties[property].IsModified = true;
                Properties[property].Value = value;
            }
            else
                Properties.Add(property, new PropertyValue(property, 0f, value));
        }

        protected void SetPropertyBaseValue(Property property, float baseValue)
        {
            if (Properties.ContainsKey(property))
            {
                if (Properties[property].BaseValue != baseValue)
                    Properties[property].IsModified = true;
                Properties[property].BaseValue = baseValue;
            }
            else
                Properties.Add(property, new PropertyValue(property, baseValue, 0f));
        }

        protected float? GetPropertyBaseValue(Property property)
        {
            return Properties.ContainsKey(property) ? Properties[property].BaseValue : default;
        }

        protected float? GetPropertyValue(Property property)
        {
            return Properties.ContainsKey(property) ? Properties[property].Value : default;
        }

        protected (float?, float?) GetProperty(Property property)
        {
            return Properties.ContainsKey(property) ? (Properties[property].Value, Properties[property].BaseValue) : default;
        }

        protected float? GetStatValue(Stat stat)
        {
            return Stats.ContainsKey(stat) ? Stats[stat].Value : default;
        }

        protected void SetStat(Stat stat, float value)
        {
            if (Stats.ContainsKey(stat))
            {
                if (Stats[stat].Value != value)
                    Stats[stat].IsModified = true;
                Stats[stat].Value = value;
            }
            else
                Stats.Add(stat, new StatValue(stat, value));

            if (Stats[stat].IsModified)
            {
                // TODO: incomplete + outsource to method
                switch(stat)
                {
                    case Stat.Level:
                        // level up
                        SetPropertyBaseValue(Property.BaseHealth, (value*200));
                        // do XP stuff etc...
                        break;
                    case Stat.Health:
                        // check if dead
                        break;
                }
            }
        }

        protected void SetStat(Stat stat, uint value)
        {
            SetStat(stat, (float)value);
        }

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="Player"/>'s in range.
        /// </summary>
        public void EnqueueToVisible(IWritable message, bool includeSelf = false)
        {
            foreach (WorldEntity entity in visibleEntities)
            {
                var player = entity as Player;
                if (player == null)
                    continue;

                if (!includeSelf && player == this)
                    continue;

                player.Session.EnqueueMessageEncrypted(message);
            }       
        }
    }
}
