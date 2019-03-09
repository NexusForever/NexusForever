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
        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }
        public uint Target { get;  set; }

        public Dictionary<ItemSlot, ItemVisual> itemVisuals = new Dictionary<ItemSlot, ItemVisual>();

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
