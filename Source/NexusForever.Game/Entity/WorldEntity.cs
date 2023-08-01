using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Entity.Movement;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Reputation;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Social;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Entity
{
    public abstract class WorldEntity : GridEntity, IWorldEntity
    {
        public EntityType Type { get; }
        public EntityCreateFlag CreateFlags { get; set; }
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public uint EntityId { get; protected set; }

        public uint CreatureId
        {
            get => CreatureEntry?.Id ?? 0;
            set
            {
                CreatureEntry = GameTableManager.Instance.Creature2.GetEntry(value);
                SetVisualEmit(true);
            }
        }

        public Creature2Entry CreatureEntry { get; private set; }

        public uint DisplayInfo
        {
            get => CreatureDisplayEntry?.Id ?? 0;
            set
            {
                CreatureDisplayEntry = GameTableManager.Instance.Creature2DisplayInfo.GetEntry(value);
                SetVisualEmit(true);
            }
        }

        public Creature2DisplayInfoEntry CreatureDisplayEntry { get; private set; }

        public ushort OutfitInfo
        {
            get => (ushort)(CreatureOutfitEntry?.Id ?? 0);
            set
            {
                CreatureOutfitEntry = GameTableManager.Instance.Creature2OutfitInfo.GetEntry(value);
                SetVisualEmit(true);
            }
        }

        public Creature2OutfitInfoEntry CreatureOutfitEntry { get; private set; }

        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }

        public ulong ActivePropId { get; private set; }
        public ushort WorldSocketId { get; private set; }

        public Vector3 LeashPosition { get; protected set; }
        public float LeashRange { get; protected set; } = 15f;
        public IMovementManager MovementManager { get; private set; }

        public virtual uint Health
        {
            get => GetStatInteger(Stat.Health) ?? 0u;
            set
            {
                SetStat(Stat.Health, Math.Clamp(value, 0u, MaxHealth)); // TODO: Confirm MaxHealth is actually the maximum health would be at.
                EnqueueToVisible(new ServerEntityHealthUpdate
                {
                    UnitId = Guid,
                    Health = Health
                });
            }
        }

        public uint MaxHealth
        {
            get => (uint)GetPropertyValue(Property.BaseHealth);
            set => SetBaseProperty(Property.BaseHealth, value);
        }

        public uint Shield
        {
            get => GetStatInteger(Stat.Shield) ?? 0u;
            set => SetStat(Stat.Shield, Math.Clamp(value, 0u, MaxShieldCapacity)); // TODO: Handle overshield
        }

        public uint MaxShieldCapacity
        {
            get => (uint)GetPropertyValue(Property.ShieldCapacityMax);
            set => SetBaseProperty(Property.ShieldCapacityMax, value);
        }

        public virtual uint Level
        {
            get => GetStatInteger(Stat.Level) ?? 1u;
            set => SetStat(Stat.Level, value);
        }

        public bool Sheathed
        {
            get => Convert.ToBoolean(GetStatInteger(Stat.Sheathed) ?? 0u);
            set => SetStat(Stat.Sheathed, Convert.ToUInt32(value));
        }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> currently targeted.
        /// </summary>
        public uint TargetGuid { get; set; }

        /// <summary>
        /// Guid of the <see cref="IPlayer"/> currently controlling this <see cref="IWorldEntity"/>.
        /// </summary>
        public uint ControllerGuid { get; set; }

        /// <summary>
        /// Initial stab at a timer to regenerate Health & Shield values.
        /// </summary>
        private UpdateTimer statUpdateTimer = new UpdateTimer(0.25); // TODO: Long-term this should be absorbed into individual timers for each Stat regeneration method

        protected readonly Dictionary<Stat, IStatValue> stats = new Dictionary<Stat, IStatValue>();

        private readonly Dictionary<Property, IPropertyValue> properties = new ();
        private readonly HashSet<Property> dirtyProperties = new();
        private bool invokeStatBalance = false;

        private bool emitVisual;
        private readonly Dictionary<ItemSlot, IItemVisual> itemVisuals = new();

        /// <summary>
        /// Create a new <see cref="IWorldEntity"/> with supplied <see cref="EntityType"/>.
        /// </summary>
        protected WorldEntity(EntityType type)
        {
            Type = type;
        }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> with supplied data.
        /// </summary>
        public void Initialise(uint creatureId, uint displayInfo, ushort outfitInfo)
        {
            CreatureId  = creatureId;
            DisplayInfo = displayInfo;
            OutfitInfo  = outfitInfo;

            SetVisualEmit(false);
        }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> with supplied data.
        /// </summary>
        public void Initialise(uint creatureId)
        {
            CreatureId = creatureId;
        }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> from an existing database model.
        /// </summary>
        public virtual void Initialise(EntityModel model)
        {
            EntityId      = model.Id;
            CreatureId    = model.Creature;
            Rotation      = new Vector3(model.Rx, model.Ry, model.Rz);
            DisplayInfo   = model.DisplayInfo;
            OutfitInfo    = model.OutfitInfo;
            Faction1      = (Faction)model.Faction1;
            Faction2      = (Faction)model.Faction2;
            ActivePropId  = model.ActivePropId;
            WorldSocketId = model.WorldSocketId;

            foreach (EntityStatModel statModel in model.EntityStat)
                stats.Add((Stat)statModel.Stat, new StatValue(statModel));

            // TODO: handle this better
            Health = MaxHealth;

            SetVisualEmit(false);
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            LeashPosition   = vector;
            MovementManager = new MovementManager(this, vector, Rotation);
            base.OnAddToMap(map, guid, vector);
        }

        public override void OnRemoveFromMap()
        {
            base.OnRemoveFromMap();
            MovementManager = null;
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            MovementManager.Update(lastTick);

            if (emitVisual)
            {
                EnqueueToVisible(BuildVisualUpdate(), true);
                SetVisualEmit(false);
            }

            statUpdateTimer.Update(lastTick);
            if (statUpdateTimer.HasElapsed)
            {
                HandleStatUpdate(lastTick);
                statUpdateTimer.Reset();
            }

            if (dirtyProperties.Count != 0)
            {
                EnqueueToVisible(BuildPropertyUpdates(), true);
                dirtyProperties.Clear();
            }
        }

        protected abstract IEntityModel BuildEntityModel();

        public virtual ServerEntityCreate BuildCreatePacket()
        {
            dirtyProperties.Clear();

            ServerEntityCreate entityCreatePacket =  new ServerEntityCreate
            {
                Guid         = Guid,
                Type         = Type,
                EntityModel  = BuildEntityModel(),
                CreateFlags  = (byte)CreateFlags,
                Stats        = stats.Values
                    .Select(s => new StatValueInitial
                    {
                        Stat  = s.Stat,
                        Type  = s.Type,
                        Value = s.Value,
                        Data  = s.Data
                    })
                    .ToList(),
                Commands     = MovementManager.ToList(),
                VisibleItems = itemVisuals
                    .Select(v => v.Value.Build())
                    .ToList(),
                Properties   = properties.Values
                    .Select(p => p.Build())
                    .ToList(),
                Faction1     = Faction1,
                Faction2     = Faction2,
                DisplayInfo  = DisplayInfo,
                OutfitInfo   = OutfitInfo
            };

            // Plugs should not have this portion of the packet set by this Class. The Plug Class should set it itself.
            // This is in large part due to the way Plugs are tied either to a DecorId OR Guid. Other entities do not have the same issue.
            if (!(this is IPlug))
            {
                if (ActivePropId > 0 || WorldSocketId > 0)
                {
                    entityCreatePacket.WorldPlacementData = new ServerEntityCreate.WorldPlacement
                    {
                        Type         = 1,
                        ActivePropId = ActivePropId,
                        SocketId     = WorldSocketId
                    };
                }
            }

            return entityCreatePacket;
        }

        // TODO: research the difference between a standard activation and cast activation

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is activated.
        /// </summary>
        public virtual void OnActivate(IPlayer activator)
        {
            // deliberately empty
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is cast activated.
        /// </summary>
        public virtual void OnActivateCast(IPlayer activator)
        {
            // deliberately empty
        }

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for <see cref="IWorldEntity"/>.
        /// </summary>
        public IEnumerable<IItemVisual> GetVisuals()
        {
            return itemVisuals.Values;
        }

        /// <summary>
        /// Set <see cref="IWorldEntity"/> to broadcast all <see cref="IItemVisual"/> on next world update.
        /// </summary>
        public void SetVisualEmit(bool status)
        {
            emitVisual = status;
        }

        /// <summary>
        /// Set visual info of <see cref="IWorldEntity"/> with supplied data.
        /// </summary>
        public void SetVisualInfo(uint displayInfo, ushort outfitInfo)
        {
            DisplayInfo = displayInfo;
            OutfitInfo = outfitInfo;
        }

        /// <summary>
        /// Add or update <see cref="IItemVisual"/> at <see cref="ItemSlot"/> with supplied data.
        /// </summary>
        public void AddVisual(ItemSlot slot, ushort displayId, ushort colourSetId = 0, int dyeData = 0)
        {
            AddVisual(new ItemVisual
            {
                Slot        = slot,
                DisplayId   = displayId,
                ColourSetId = colourSetId,
                DyeData     = dyeData
            });
        }

        /// <summary>
        /// Add or update <see cref="IItemVisual"/>.
        /// </summary>
        public virtual void AddVisual(IItemVisual visual)
        {
            if (!itemVisuals.ContainsKey(visual.Slot))
                itemVisuals.Add(visual.Slot, visual);
            else
                itemVisuals[visual.Slot] = visual;

            SetVisualEmit(true);
        }

        /// <summary>
        /// Remove <see cref="IItemVisual"/> at supplied <see cref="ItemSlot"/>.
        /// </summary>
        public void RemoveVisual(ItemSlot slot)
        {
            itemVisuals.Remove(slot);
            SetVisualEmit(true);
        }

        protected virtual ServerEntityVisualUpdate BuildVisualUpdate()
        {
            return new ServerEntityVisualUpdate
            {
                UnitId      = Guid,
                CreatureId  = CreatureId,
                DisplayInfo = DisplayInfo,
                OutfitInfo  = OutfitInfo,
                ItemVisuals = itemVisuals.Values
                    .Select(v => v.Build())
                    .ToList()
            };
        }

        /// <summary>
        /// Return a collection of <see cref="IPropertyValue"/> for <see cref="IWorldEntity"/>.
        /// </summary>
        public IEnumerable<IPropertyValue> GetProperties()
        {
            return properties.Values;
        }

        private IPropertyValue CreateProperty(Property property, float defaultValue)
        {
            IPropertyValue propertyValue = new PropertyValue(property, defaultValue);
            properties.Add(property, propertyValue);

            return propertyValue;
        }

        /// <summary>
        /// Get <see cref="IPropertyValue"/> for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Property"/> doesn't exist it will be created with the default value specified in the GameTable.
        /// </remarks>
        public IPropertyValue GetProperty(Property property)
        {
            if (!properties.TryGetValue(property, out IPropertyValue propertyValue))
            {
                float defaultValue = GameTableManager.Instance.UnitProperty2.GetEntry((ulong)property)?.DefaultValue ?? 0f;
                propertyValue = CreateProperty(property, defaultValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Get <see cref="IPropertyValue"/> for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Property"/> doesn't exist it will be created with the default value specified.
        /// </remarks>
        public IPropertyValue GetProperty(Property property, float defaultValue)
        {
            if (!properties.TryGetValue(property, out IPropertyValue propertyValue))
                propertyValue = CreateProperty(property, defaultValue);

            return propertyValue;
        }

        /// <summary>
        /// Returns the base value for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        public float GetPropertyBaseValue(Property property)
        {
            return GetProperty(property).BaseValue;
        }

        /// <summary>
        /// Returns the primary value for <see cref="IWorldEntity"/> <see cref="Property"/>.
        /// </summary>
        public float GetPropertyValue(Property property)
        {
            return GetProperty(property).Value;
        }

        /// <summary>
        /// Sets the base value and calculate primary value for <see cref="Property"/>.
        /// </summary>
        public void SetBaseProperty(Property property, float value)
        {
            IPropertyValue propertyValue = GetProperty(property, value);
            propertyValue.BaseValue = value;

            CalculateProperty(propertyValue);
        }

        /// <summary>
        /// Calculate the primary value for <see cref="Property"/>.
        /// </summary>
        public void CalculateProperty(Property property)
        {
            IPropertyValue propertyValue = GetProperty(property);
            CalculateProperty(propertyValue);
        }

        /// <summary>
        /// Calculate the primary value for <see cref="Property"/> and set delayed emit.
        /// </summary>
        private void CalculateProperty(IPropertyValue propertyValue)
        {
            if (propertyValue == null)
                throw new ArgumentNullException(nameof(propertyValue));

            #if DEBUG
            float previousValue = propertyValue.Value;
            #endif

            CalculatePropertyValue(propertyValue);
            SetPropertyEmit(propertyValue.Property);

            DependantStatBalance(propertyValue);

            OnPropertyUpdate(propertyValue);

            #if DEBUG
            if (this is IPlayer player && !player.IsLoading)
                player.SendSystemMessage($"Property {propertyValue.Property} changing, base: {propertyValue.BaseValue}, previous: {previousValue}, new: {propertyValue.Value}.");
            #endif
        }

        /// <summary>
        /// Calculate the primary value for <see cref="Property"/>.
        /// </summary>
        protected virtual void CalculatePropertyValue(IPropertyValue propertyValue)
        {
            propertyValue.Value = propertyValue.BaseValue;
        }

        /// <summary>
        /// Set <see cref="IWorldEntity"/> to broadcast <see cref="Property"/> on next world update.
        /// </summary>
        public void SetPropertyEmit(Property property)
        {
            dirtyProperties.Add(property);
        }

        protected void SetDependantStatBalance(bool value)
        {
            invokeStatBalance = value;
        }

        protected void DependantStatBalance(IPropertyValue propertyValue)
        {
            if (!invokeStatBalance)
                return;

            switch (propertyValue.Property)
            {
                case Property.BaseHealth:
                    if (propertyValue.Value < Health)
                        Health = MaxHealth;
                    break;
                case Property.ShieldCapacityMax:
                    if (propertyValue.Value < Shield)
                        Shield = MaxShieldCapacity;
                    break;
            }
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> has a <see cref="Property"/> updated.
        /// </summary>
        protected virtual void OnPropertyUpdate(IPropertyValue propertyValue)
        {
            // deliberately empty
        }

        /// <summary>
        /// Used to build the <see cref="ServerEntityPropertiesUpdate"/> from all modified <see cref="Property"/>.
        /// </summary>
        private IWritable BuildPropertyUpdates()
        {
            return new ServerEntityPropertiesUpdate()
            {
                UnitId     = Guid,
                Properties = dirtyProperties
                    .Select(p => properties[p].Build())
                    .ToList()
            };
        }

        /// <summary>
        /// Return the <see cref="float"/> value of the supplied <see cref="Stat"/>.
        /// </summary>
        protected float? GetStatFloat(Stat stat)
        {
            StatAttribute attribute = EntityManager.Instance.GetStatAttribute(stat);
            if (attribute?.Type != StatType.Float)
                throw new ArgumentException();

            if (!stats.TryGetValue(stat, out IStatValue statValue))
                return null;

            return statValue.Value;
        }

        /// <summary>
        /// Return the <see cref="uint"/> value of the supplied <see cref="Stat"/>.
        /// </summary>
        protected uint? GetStatInteger(Stat stat)
        {
            StatAttribute attribute = EntityManager.Instance.GetStatAttribute(stat);
            if (attribute?.Type != StatType.Integer)
                throw new ArgumentException();

            if (!stats.TryGetValue(stat, out IStatValue statValue))
                return null;

            return (uint)statValue.Value;
        }

        /// <summary>
        /// Return the <see cref="uint"/> value of the supplied <see cref="Stat"/> as an <see cref="Enum"/>.
        /// </summary>
        public T? GetStatEnum<T>(Stat stat) where T : struct, Enum
        {
            uint? value = GetStatInteger(stat);
            if (value == null)
                return null;

            return (T)Enum.ToObject(typeof(T), value.Value);
        }

        /// <summary>
        /// Set <see cref="Stat"/> to the supplied <see cref="float"/> value.
        /// </summary>
        protected void SetStat(Stat stat, float value)
        {
            StatAttribute attribute = EntityManager.Instance.GetStatAttribute(stat);
            if (attribute?.Type != StatType.Float)
                throw new ArgumentException();

            if (stats.TryGetValue(stat, out IStatValue statValue))
                statValue.Value = value;
            else
            {
                statValue = new StatValue(stat, value);
                stats.Add(stat, statValue);
            }

            if (attribute.SendUpdate)
            {
                EnqueueToVisible(new ServerEntityStatUpdateFloat
                {
                    UnitId = Guid,
                    Stat   = new StatValueUpdate
                    {
                        Stat  = statValue.Stat,
                        Type  = statValue.Type,
                        Value = statValue.Value
                    }
                }, true);
            }
        }

        /// <summary>
        /// Set <see cref="Stat"/> to the supplied <see cref="uint"/> value.
        /// </summary>
        protected void SetStat(Stat stat, uint value)
        {
            StatAttribute attribute = EntityManager.Instance.GetStatAttribute(stat);
            if (attribute?.Type != StatType.Integer)
                throw new ArgumentException();

            if (stats.TryGetValue(stat, out IStatValue statValue))
                statValue.Value = value;
            else
            {
                statValue = new StatValue(stat, value);
                stats.Add(stat, statValue);
            }

            if (attribute.SendUpdate)
            {
                EnqueueToVisible(new ServerEntityStatUpdateInteger
                {
                    UnitId = Guid,
                    Stat   = new StatValueUpdate
                    {
                        Stat  = statValue.Stat,
                        Type  = statValue.Type,
                        Value = statValue.Value
                    }
                }, true);
            }
        }

        /// <summary>
        /// Set <see cref="Stat"/> to the supplied <see cref="Enum"/> value.
        /// </summary>
        protected void SetStat<T>(Stat stat, T value) where T : Enum, IConvertible
        {
            SetStat(stat, value.ToUInt32(null));
        }

        /// <summary>
        /// Handles regeneration of Stat Values. Used to provide a hook into the Update method, for future implementation.
        /// </summary>
        private void HandleStatUpdate(double lastTick)
        {
            // TODO: This should probably get moved to a Calculation Library/Manager at some point. There will be different timers on Stat refreshes, but right now the timer is hardcoded to every 0.25s.
            // Probably worth considering an Attribute-grouped Class that allows us to run differentt regeneration methods & calculations for each stat.

            if (Health < MaxHealth)
                Health += (uint)(MaxHealth / 200f);

            if (Shield < MaxShieldCapacity)
                Shield += (uint)(MaxShieldCapacity * GetPropertyValue(Property.ShieldRegenPct) * statUpdateTimer.Duration);
        }

        /// <summary>
        /// Enqueue broadcast of <see cref="IWritable"/> to all visible <see cref="IPlayer"/>'s in range.
        /// </summary>
        public void EnqueueToVisible(IWritable message, bool includeSelf = false)
        {
            foreach (IGridEntity entity in visibleEntities.Values)
            {
                if (entity is not IPlayer player)
                    continue;

                if (!includeSelf && (Guid == entity.Guid || ControllerGuid == entity.Guid))
                    continue;

                player.Session.EnqueueMessageEncrypted(message);
            }
        }

        /// <summary>
        /// Return <see cref="Disposition"/> between <see cref="IWorldEntity"/> and <see cref="Faction"/>.
        /// </summary>
        public virtual Disposition GetDispositionTo(Faction factionId, bool primary = true)
        {
            IFactionNode targetFaction = FactionManager.Instance.GetFaction(factionId);
            if (targetFaction == null)
                throw new ArgumentException($"Invalid faction {factionId}!");

            // find disposition based on faction friendships
            Disposition? dispositionFromFactionTarget = GetDispositionFromFactionFriendship(targetFaction, primary ? Faction1 : Faction2);
            if (dispositionFromFactionTarget.HasValue)
                return dispositionFromFactionTarget.Value;

            IFactionNode invokeFaction = FactionManager.Instance.GetFaction(primary ? Faction1 : Faction2);
            Disposition? dispositionFromFactionInvoker = GetDispositionFromFactionFriendship(invokeFaction, factionId);
            if (dispositionFromFactionInvoker.HasValue)
                return dispositionFromFactionInvoker.Value;

            // TODO: client does a few more checks, might not be 100% accurate

            // default to neutral if we have no disposition from other sources
            return Disposition.Neutral;
        }

        private Disposition? GetDispositionFromFactionFriendship(IFactionNode node, Faction factionId)
        {
            if (node == null)
                return null;

            // check if current node has required friendship
            FactionLevel? level = node.GetFriendshipFactionLevel(factionId);
            if (level.HasValue)
                return FactionNode.GetDisposition(level.Value);

            // check if parent node has required friendship
            return GetDispositionFromFactionFriendship(node.Parent, factionId);
        }

        private IChatMessageBuilder BuildNpcChat(string text, ChatChannelType type)
        {
            return new ChatMessageBuilder
            {
                Type     = type,
                Text     = text,
                Guid     = Guid,
                // TODO: should this be based on the players session language?
                FromName = GameTableManager.Instance.TextEnglish.GetEntry(CreatureEntry.LocalizedTextIdName)
            };
        }

        /// <summary>
        /// Broadcast NPC say chat message to to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        public void NpcSay(string text, float range = 155f)
        {
            if (CreatureEntry == null)
                return;

            Talk(BuildNpcChat(text, ChatChannelType.NPCSay), range);
        }

        /// <summary>
        /// Broadcast NPC yell chat message to to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        public void NpcYell(string text, float range = 310f)
        {
            if (CreatureEntry == null)
                return;

            Talk(BuildNpcChat(text, ChatChannelType.NPCYell), range);
        }

        /// <summary>
        /// Broadcast chat message built from <see cref="IChatMessageBuilder"/> to <see cref="IPlayer"/> in supplied range.
        /// </summary>
        public void Talk(IChatMessageBuilder builder, float range, IGridEntity exclude = null)
        {
            if (Map == null)
                throw new InvalidOperationException();

            Map.Search(
                Position,
                range,
                new SearchCheckRangePlayerOnly(Position, range, exclude),
                out List<IGridEntity> intersectedEntities);

            IWritable message = builder.Build();
            foreach (IPlayer player in intersectedEntities.Cast<IPlayer>())
                player.Session.EnqueueMessageEncrypted(message);
        }
    }
}
