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
using NetworkPropertyValue = NexusForever.Network.World.Message.Model.Shared.PropertyValue;

namespace NexusForever.Game.Entity
{
    public abstract class WorldEntity : GridEntity, IWorldEntity
    {
        public EntityType Type { get; }
        public EntityCreateFlag CreateFlags { get; set; }
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        /// <summary>
        /// Property related cached data
        /// </summary>
        public Dictionary<Property, IPropertyValue> Properties { get; } = new Dictionary<Property, IPropertyValue>();
        private Dictionary<Property, float> BaseProperties { get; } = new Dictionary<Property, float>();
        private Dictionary<Property, Dictionary<ItemSlot, /*value*/float>> ItemProperties { get; } = new Dictionary<Property, Dictionary<ItemSlot, float>>();
        private Dictionary<Property, Dictionary</*spell4Id*/uint, ISpellPropertyModifier>> SpellProperties { get; } = new Dictionary<Property, Dictionary<uint, ISpellPropertyModifier>>();
        private HashSet<Property> DirtyProperties { get; } = new HashSet<Property>();

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

        public uint Health
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
                if (this is IPlayer player)
                    player.Session.EnqueueMessageEncrypted(new ServerPlayerHealthUpdate
                    {
                        UnitId = Guid,
                        Health = Health,
                        Mask = (UpdateHealthMask)4
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

        public uint Level
        {
            get => GetStatInteger(Stat.Level) ?? 1u;
            set
            {
                SetStat(Stat.Level, value);
                if (this is Player player)
                    player.BuildBaseProperties();
            }
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

            SetVisualEmit(false);

            BuildBaseProperties();
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

            if (!HasPendingPropertyChanges)
                return;

            EnqueueToVisible(BuildPropertyUpdates(), true);
        }

        protected abstract IEntityModel BuildEntityModel();

        public virtual ServerEntityCreate BuildCreatePacket()
        {
            DirtyProperties.Clear();

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
                Properties   = Properties.Values
                    .Select(p => new NetworkPropertyValue
                    {
                        Property  = p.Property,
                        BaseValue = p.BaseValue,
                        Value     = p.Value
                    })
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
        /// Used to build the <see cref="ServerEntityPropertiesUpdate"/> from all modified <see cref="Property"/>
        /// </summary>
        private ServerEntityPropertiesUpdate BuildPropertyUpdates()
        {
            if (!HasPendingPropertyChanges)
                return null;
            
            ServerEntityPropertiesUpdate propertyUpdatePacket = new ServerEntityPropertiesUpdate()
            {
                UnitId = Guid
            };
            
            foreach (Property propertyUpdate in DirtyProperties)
            {
                IPropertyValue propertyValue = CalculateProperty(propertyUpdate);
                if (Properties.ContainsKey(propertyUpdate))
                    Properties[propertyUpdate] = propertyValue;
                else
                    Properties.Add(propertyUpdate, propertyValue);

                OnPropertyUpdate(propertyUpdate, propertyValue.Value);

                propertyUpdatePacket.Properties.Add(propertyValue.Build());
            }

            DirtyProperties.Clear();
            return propertyUpdatePacket;
        }

        /// <summary>
        /// Calculates and builds a <see cref="PropertyValue"/> for this Entity's <see cref="Property"/>
        /// </summary>
        private IPropertyValue CalculateProperty(Property property)
        {
            float baseValue = GetBasePropertyValue(property);
            float value = baseValue;
            baseValue = GameTableManager.Instance.UnitProperty2.GetEntry((ulong)property).DefaultValue;
            float itemValue = 0f;

            float originalValue = value;

            // Run through spell adjustments first because they could adjust base properties
            // dataBits01 appears to be some form of Priority or Math Operator
            foreach (ISpellPropertyModifier spellModifier in GetSpellPropertyModifiers(property).OrderByDescending(s => s.Priority))
            {
                foreach (IPropertyModifier alteration in spellModifier.Alterations)
                {
                    // TODO: Add checks to ensure we're not modifying FlatValue and Percentage in the same effect?

                    _ = alteration.ModType switch
                    {
                        ModType.FlatValue  => value += alteration.Value,
                        ModType.Percentage => value *= alteration.Value,
                        ModType.LevelScale => value += alteration.Value * Level,
                    };
                }
            }

            foreach (KeyValuePair<ItemSlot, float> itemStats in GetItemProperties(property))
                itemValue += itemStats.Value;

            value += itemValue;

#if DEBUG
            if (this is IPlayer player && !player.IsLoading)
                player.SendSystemMessage($"Property {property} changing from {originalValue} to {value}.");
#endif

            return new PropertyValue(property, baseValue, value);
        }

        /// <summary>
        /// Used on entering world to set the <see cref="WorldEntity"/> base <see cref="PropertyValue"/>
        /// </summary>
        public virtual void BuildBaseProperties()
        {
            ServerEntityPropertiesUpdate propertiesUpdate = BuildPropertyUpdates();

            if (this is not IPlayer player)
                return;

            if (!player.IsLoading)
                player.EnqueueToVisible(propertiesUpdate, true);
        }

        public bool HasPendingPropertyChanges => DirtyProperties.Count != 0;

        /// <summary>
        /// Sets the base value for a <see cref="Property"/>
        /// </summary>
        public void SetBaseProperty(Property property, float value)
        {
            if (BaseProperties.ContainsKey(property))
                BaseProperties[property] = value;
            else
                BaseProperties.Add(property, value);

            DirtyProperties.Add(property);
        }

        /// <summary>
        /// Add a <see cref="Property"/> modifier given a Spell4Id and <see cref="PropertyModifier"/> instance
        /// </summary>
        public void AddItemProperty(Property property, ItemSlot itemSlot, float value)
        {
            if (ItemProperties.ContainsKey(property))
            {
                var itemDict = ItemProperties[property];

                if (itemDict.ContainsKey(itemSlot))
                    itemDict[itemSlot] = value;
                else
                    itemDict.Add(itemSlot, value);
            }
            else
            {
                ItemProperties.Add(property, new Dictionary<ItemSlot, float>
                {
                    { itemSlot, value }
                });
            }

            DirtyProperties.Add(property);
        }

        /// <summary>
        /// Remove a <see cref="Property"/> modifier by a Spell that is currently affecting this <see cref="WorldEntity"/>
        /// </summary>
        public void RemoveItemProperty(Property property, ItemSlot itemSlot)
        {
            if (ItemProperties.ContainsKey(property))
            {
                var itemDict = ItemProperties[property];

                if (itemDict.ContainsKey(itemSlot))
                    itemDict.Remove(itemSlot);
            }

            DirtyProperties.Add(property);
        }

        /// <summary>
        /// Add a <see cref="Property"/> modifier given a Spell4Id and <see cref="IPropertyModifier"/> instance
        /// </summary>
        public void AddSpellModifierProperty(ISpellPropertyModifier spellModifier, uint spell4Id)
        {
            if (SpellProperties.ContainsKey(spellModifier.Property))
            {
                var spellDict = SpellProperties[spellModifier.Property];

                if (spellDict.ContainsKey(spell4Id))
                    spellDict[spell4Id] = spellModifier;
                else
                    spellDict.Add(spell4Id, spellModifier);
            }
            else
            {
                SpellProperties.Add(spellModifier.Property, new Dictionary<uint, ISpellPropertyModifier>
                {
                    { spell4Id, spellModifier }
                });
            }

            DirtyProperties.Add(spellModifier.Property);
        }

        /// <summary>
        /// Remove a <see cref="Property"/> modifier by a Spell that is currently affecting this <see cref="WorldEntity"/>
        /// </summary>
        public void RemoveSpellProperty(Property property, uint spell4Id)
        {
            if (SpellProperties.ContainsKey(property))
        {
                var spellDict = SpellProperties[property];

                if (spellDict.ContainsKey(spell4Id))
                    spellDict.Remove(spell4Id);
            }

            DirtyProperties.Add(property);
        }

        /// <summary>
        /// Remove all <see cref="Property"/> modifiers by a Spell that is currently affecting this <see cref="WorldEntity"/>
        /// </summary>
        public void RemoveSpellProperties(uint spell4Id)
        {
            List<Property> propertiesWithSpell = SpellProperties.Where(i => i.Value.Keys.Contains(spell4Id)).Select(p => p.Key).ToList();

            foreach (Property property in propertiesWithSpell)
                RemoveSpellProperty(property, spell4Id);
        }

        /// <summary>
        /// Return the base value for this <see cref="WorldEntity"/>'s <see cref="Property"/>
        /// </summary>
        private float GetBasePropertyValue(Property property)
        {
            return BaseProperties.ContainsKey(property) ? BaseProperties[property] : GameTableManager.Instance.UnitProperty2.GetEntry((ulong)property).DefaultValue;
        }

        /// <summary>
        /// Return all item property values for this <see cref="WorldEntity"/>'s <see cref="Property"/>
        /// </summary>
        private Dictionary<ItemSlot, float> GetItemProperties(Property property)
        {
            return ItemProperties.TryGetValue(property, out Dictionary<ItemSlot, float> properties) ? properties : new Dictionary<ItemSlot, float>();
        }

        /// <summary>
        /// Return all <see cref="IPropertyModifier"/> for this <see cref="WorldEntity"/>'s <see cref="Property"/>
        /// </summary>
        private IEnumerable<ISpellPropertyModifier> GetSpellPropertyModifiers(Property property)
        {
            return SpellProperties.ContainsKey(property) ? SpellProperties[property].Values : Enumerable.Empty<ISpellPropertyModifier>();
        }

        /// <summary>
        /// Returns the current value for this <see cref="WorldEntity"/>'s <see cref="Property"/>
        /// </summary>
        public float GetPropertyValue(Property property)
        {
            return Properties.ContainsKey(property) ? Properties[property].Value : default;
        }

        /// <summary>
        /// Invoked when <see cref="WorldEntity"/> has a <see cref="Property"/> updated.
        /// </summary>
        protected virtual void OnPropertyUpdate(Property property, float newValue)
        {
            switch (property)
            {
                case Property.BaseHealth:
                    if (newValue < Health)
                        Health = MaxHealth;
                    break;
                case Property.ShieldCapacityMax:
                    if (newValue < Shield)
                        Shield = MaxShieldCapacity;
                    break;
            }
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
