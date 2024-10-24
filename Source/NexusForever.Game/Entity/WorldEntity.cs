using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.CSI;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Reputation;
using NexusForever.Game.Social;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Social;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Static;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Script;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Collection;

namespace NexusForever.Game.Entity
{
    public abstract class WorldEntity : GridEntity, IWorldEntity
    {
        public abstract EntityType Type { get; }
        public EntityCreateFlag CreateFlags { get; set; }

        public Vector3 Rotation
        {
            get => MovementManager.GetRotation();
            set => MovementManager.SetRotation(value, false);
        }

        public WorldZoneEntry Zone { get; private set; }
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

        public byte QuestChecklistIdx { get; private set; }

        public ulong ActivePropId { get; private set; }
        public ushort WorldSocketId { get; private set; }

        public EntitySplineModel Spline { get; private set; }

        public Vector3 LeashPosition { get; protected set; }
        public float LeashRange { get; protected set; } = 15f;
        public IMovementManager MovementManager { get; private set; }

        public virtual uint Health
        {
            get => GetStatInteger(Stat.Health) ?? 0u;
            protected set
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

        public StandState StandState
        {
            get => (StandState)(GetStatInteger(Stat.StandState) ?? 0u);
            set
            {
                SetStat(Stat.StandState, (uint)value);

                EnqueueToVisible(new ServerEmote
                {
                    Guid       = Guid,
                    StandState = value
                });
            }
        }

        /// <summary>
        /// Collection of guids currently targeting this <see cref="IWorldEntity"/>.
        /// </summary>
        public IEnumerable<uint> TargetingGuids => targetingGuids;

        private readonly HashSet<uint> targetingGuids = new();

        /// <summary>
        /// Guid of the <see cref="IPlayer"/> currently controlling this <see cref="IWorldEntity"/>.
        /// </summary>
        public uint? ControllerGuid
        {
            get => controllerGuid;
            set
            {
                controllerGuid = value;
                MovementManager.ServerControl = value == null;
            }
        }

        private uint? controllerGuid;

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> the <see cref="IWorldEntity"/> is a passenger on.
        /// </summary>
        public uint? PlatformGuid
        {
            get => MovementManager.GetPlatform();
            private set => MovementManager.SetPlatform(value);
        }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> that summoned this <see cref="IWorldEntity"/>.
        /// </summary>
        public uint? SummonerGuid { get; set; }

        /// <summary>
        /// An entity factory to summon child entities.
        /// </summary>
        /// <remarks>
        /// Any entities summoned by this <see cref="IWorldEntity"/> will be removed when this <see cref="IWorldEntity"/> is removed.
        /// </remarks>
        public IEntitySummonFactory SummonFactory { get; private set; }

        /// <summary>
        /// Collection of guids currently passengers on this <see cref="IWorldEntity"/>.
        /// </summary>
        public IEnumerable<uint> PlatformPassengerGuids => platformPassengerGuids;
        private readonly HashSet<uint> platformPassengerGuids = new();

        protected readonly Dictionary<Stat, IStatValue> stats = new Dictionary<Stat, IStatValue>();

        private readonly Dictionary<Property, IPropertyValue> properties = new ();
        private readonly HashSet<Property> dirtyProperties = new();
        private bool invokeStatBalance = false;

        private bool emitVisual;
        private readonly Dictionary<ItemSlot, IItemVisual> itemVisuals = new();

        #region Dependency Injection

        public WorldEntity(
            IMovementManager movementManager,
            IEntitySummonFactory summonFactory)
        {
            MovementManager = movementManager;
            MovementManager.Initialise(this);

            SummonFactory = summonFactory;
            SummonFactory.Initialise(this);
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> with supplied data.
        /// </summary>
        public void Initialise(uint creatureId)
        {
            CreatureId = creatureId;

            CalculateDefaultProperties();

            // TODO: handle this better
            Health = MaxHealth;
            Shield = MaxShieldCapacity;

            InitialiseScriptCollection(null);
        }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> from an existing database model.
        /// </summary>
        public virtual void Initialise(EntityModel model)
        {
            EntityId          = model.Id;
            CreatureId        = model.Creature;
            Rotation          = new Vector3(model.Rx, model.Ry, model.Rz);
            DisplayInfo       = model.DisplayInfo;
            OutfitInfo        = model.OutfitInfo;
            Faction1          = (Faction)model.Faction1;
            Faction2          = (Faction)model.Faction2;
            QuestChecklistIdx = model.QuestChecklistIdx;
            ActivePropId      = model.ActivePropId;
            WorldSocketId     = model.WorldSocketId;
            Spline            = model.EntitySpline;

            foreach (EntityStatModel statModel in model.EntityStat)
                stats.Add((Stat)statModel.Stat, new StatValue(statModel));

            CalculateDefaultProperties();

            foreach (EntityPropertyModel propertyModel in model.EntityProperty)
                SetBaseProperty(propertyModel.Property, propertyModel.Value);

            // TODO: handle this better
            Health = MaxHealth;
            Shield = MaxShieldCapacity;

            List<string> scriptNames = model.EntityScript
                .Select(e => e.ScriptName)
                .ToList();
            InitialiseScriptCollection(scriptNames);
        }

        /// <summary>
        /// Initialise <see cref="IScriptCollection"/> for <see cref="IWorldEntity"/>.
        /// </summary>
        protected override void InitialiseScriptCollection(List<string> names)
        {
            scriptCollection = ScriptManager.Instance.InitialiseOwnedCollection<IWorldEntity>(this);
            ScriptManager.Instance.InitialiseEntityScripts<IWorldEntity>(scriptCollection, this, names);
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            LeashPosition = vector;
            MovementManager.SetPosition(vector, false);

            base.OnAddToMap(map, guid, vector);
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is removed from <see cref="IBaseMap"/>.
        /// </summary>
        protected override void OnRemoveFromMap()
        {
            foreach (uint platformPassengerGuid in platformPassengerGuids.ToList())
            {
                IWorldEntity worldEntity = Map.GetEntity<IWorldEntity>(platformPassengerGuid);
                if (worldEntity == null)
                    continue;

                worldEntity.SetPlatform(null);
                worldEntity.MovementManager.SetPosition(Position, false);
                worldEntity.MovementManager.SetRotation(Rotation, false);
            }

            SummonFactory.Unsummon();
            if (SummonerGuid.HasValue)
            {
                IWorldEntity summoner = Map.GetEntity<IWorldEntity>(SummonerGuid.Value);
                summoner?.SummonFactory.UntrackSummon(Guid);
            }

            base.OnRemoveFromMap();
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is relocated.
        /// </summary>
        protected override void OnRelocate(Vector3 vector)
        {
            base.OnRelocate(vector);

            uint? worldAreaId = Map.File.GetWorldAreaId(vector);
            if (worldAreaId.HasValue && Zone?.Id != worldAreaId)
            {
                Zone = GameTableManager.Instance.WorldZone.GetEntry(worldAreaId.Value);
                if (Zone != null)
                {
                    OnZoneUpdate();
                    scriptCollection?.Invoke<IWorldEntityScript>(s => s.OnEnterZone(this, Zone.Id));
                }
            }
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

            if (dirtyProperties.Count != 0)
            {
                EnqueueToVisible(BuildPropertyUpdates(), true);
                dirtyProperties.Clear();
            }
        }

        protected abstract IEntityModel BuildEntityModel();

        public virtual ServerEntityCreate BuildCreatePacket(bool isLoading)
        {
            var entityCreatePacket = new ServerEntityCreate
            {
                Guid         = Guid,
                Type         = Type,
                EntityModel  = BuildEntityModel(),
                CreateFlags  = CreateFlags,
                Stats        = stats.Values
                    .Select(s => new StatValueInitial
                    {
                        Stat  = s.Stat,
                        Type  = s.Type,
                        Value = s.Value,
                        Data  = s.Data
                    })
                    .ToList(),
                Time         = MovementManager.GetTime(),
                Commands     = (isLoading && MovementManager.RequiresSynchronisation ? MovementManager.GetInitialNetworkEntityCommands() : MovementManager.GetNetworkEntityCommands()).ToList(),
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
            if (!(this is IPlugEntity))
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
        /// Invoked when <see cref="WorldEntity"/> is cast activated.
        /// </summary>
        public virtual void OnActivateCast(IPlayer activator, uint interactionId)
        {
            // Handle CSI
            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);

            uint spell4Id = 0;
            for (int i = 0; i < entry.Spell4IdActivate.Length; i++)
            {
                if (spell4Id > 0u || i == entry.Spell4IdActivate.Length)
                    break;

                if (entry.PrerequisiteIdActivateSpells[i] > 0 && PrerequisiteManager.Instance.Meets(activator, entry.PrerequisiteIdActivateSpells[i]))
                    spell4Id = entry.Spell4IdActivate[i];

                if (spell4Id == 0u && entry.Spell4IdActivate[i] == 0u && i > 0)
                    spell4Id = entry.Spell4IdActivate[i - 1];
            }

            if (spell4Id == 0)
                throw new InvalidOperationException($"Spell4Id should not be 0. Unhandled Creature ActivateCast {CreatureId}");

            SpellParameters parameters = new SpellParameters
            {
                PrimaryTargetId        = Guid,
                ClientSideInteraction  = new ClientSideInteraction(activator, this, interactionId),
                CastTimeOverride       = (int)entry.ActivateSpellCastTime,
                UserInitiatedSpellCast = true
            };
            activator.CastSpell(spell4Id, parameters);
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/>'s activate succeeds.
        /// </summary>
        public virtual void OnActivateSuccess(IPlayer activator)
        {
            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, CreatureId, 1u);
            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroupChecklist, CreatureId, QuestChecklistIdx);
            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, CreatureId, 1u);

            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(CreatureId))
            {
                activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate

                Map.PublicEventManager.UpdateObjective(activator, PublicEventObjectiveType.ActivateTargetGroup, targetGroupId, 1);
                Map.PublicEventManager.UpdateObjective(activator, PublicEventObjectiveType.ActivateTargetGroupChecklist, targetGroupId, QuestChecklistIdx);
            }

            scriptCollection?.Invoke<IWorldEntityScript>(s => s.OnActivateSuccess(activator));
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/>'s activation fails.
        /// </summary>
        public virtual void OnActivateFail(IPlayer activator)
        {
            scriptCollection?.Invoke<IWorldEntityScript>(s => s.OnActivateFail(activator));
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
            // don't broadcast visual changes if not in world, visuals will be sent with creation packet.
            if (!InWorld)
                return;

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
                propertyValue = new PropertyValue(property, CalculateDefaultProperty(property));
                properties.Add(property, propertyValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Calculate default property value for supplied <see cref="Property"/>.
        /// </summary>
        /// <remarks>
        /// Default property values are not sent to the client, they are also calculated by the client and are replaced by any property updates.
        /// </remarks>
        protected virtual float CalculateDefaultProperty(Property property)
        {
            UnitProperty2Entry entry = GameTableManager.Instance.UnitProperty2.GetEntry((uint)property);
            if (entry == null)
                return 0f;

            float value = entry.DefaultValue;
            if ((entry.Flags & UnitPropertyFlags.Static) != 0)
                return value;

            if (Type == EntityType.Pet)
                return value;

            // TODO: client also includes mentor level
            uint level = Level;
            /*if (MentorLevel.HasValue)
                level = MentorLevel.Value;
            else
                level = Level;*/

            Property levelProperty = property;
            if (property >= Property.MoveSpeedMultiplier)
            {
                // final row is used for anything above property 100
                level = (uint)GameTableManager.Instance.CreatureLevel.Entries.Length;

                // creature level entry only has 100 columns for properties, wrap around for higher values
                levelProperty = (Property)(property - Property.MoveSpeedMultiplier);
            }

            CreatureLevelEntry levelEntry = GameTableManager.Instance.CreatureLevel.GetEntry(level);
            if (levelEntry == null)
                return entry.DefaultValue;

            return levelEntry.UnitPropertyValue[(uint)levelProperty];
        }

        protected void CalculateDefaultProperties()
        {
            foreach (Property property in Enum.GetValues<Property>())
                SetBaseProperty(property, CalculateDefaultProperty(property));
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
            IPropertyValue propertyValue = GetProperty(property);
            propertyValue.BaseValue = value;

            CalculateProperty(propertyValue);
        }

        /// <summary>
        /// Calculate the primary value for <see cref="Property"/>.
        /// </summary>
        protected void CalculateProperty(Property property)
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
            // don't broadcast property changes if not in world, properties will be sent with creation packet.
            if (!InWorld)
                return;

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
        /// Set primary faction to supplied <see cref="Faction"/>.
        /// </summary>
        public void SetFaction(Faction factionId)
        {
            Faction1 = factionId;

            EnqueueToVisible(new ServerEntityFaction
            {
                UnitId  = Guid,
                Faction = factionId
            });
        }

        /// <summary>
        /// Set temporary faction to supplied <see cref="Faction"/>.
        /// </summary>
        public void SetTemporaryFaction(Faction factionId)
        {
            if (Faction1 != Faction2)
                throw new InvalidOperationException();

            Faction2 = Faction1;
            SetFaction(factionId);
        }

        /// <summary>
        /// Remove temporary faction and revert to primary faction.
        /// </summary>
        public void RemoveTemporaryFaction()
        {
            if (Faction1 == Faction2)
                throw new InvalidOperationException();

            SetFaction(Faction2);
            Faction2 = Faction1;
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
        public void Talk(IChatMessageBuilder builder, float range, IPlayer exclude = null)
        {
            if (Map == null)
                throw new InvalidOperationException();

            var check = new SearchCheckRange<IPlayer>();
            check.Initialise(Position, range);

            IEnumerable<IPlayer> players = Map.Search(Position, range, check)
                .Where(e => e != exclude);

            IWritable message = builder.Build();
            foreach (IPlayer player in players)
                player.Session.EnqueueMessageEncrypted(message);
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is targeted by another <see cref="IUnitEntity"/>.
        /// </summary>
        /// <remarks>
        /// While any entity can be targeted, only <see cref="IUnitEntity"/> can target.
        /// </remarks>
        public virtual void OnTargeted(IUnitEntity source)
        {
            targetingGuids.Add(source.Guid);
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/> is untargeted by another <see cref="IUnitEntity"/>.
        /// </summary>
        public virtual void OnUntargeted(IUnitEntity source)
        {
            targetingGuids.Remove(source.Guid);
        }

        /// <summary>
        /// Set platform to suppled <see cref="IWorldEntity"/> with optional position and rotation offsets.
        /// </summary>
        public void SetPlatform(IWorldEntity entity, Vector3 position = default, Vector3 rotation = default)
        {
            if (PlatformGuid != null)
            {
                IWorldEntity platform = Map.GetEntity<IWorldEntity>(PlatformGuid.Value);
                platform?.RemovePlatformPassenger(this);

                MovementManager.SetPosition(Position, false);
                MovementManager.SetRotation(Rotation, false);
            }

            PlatformGuid = entity?.Guid;

            if (entity != null)
            {
                entity.AddPlatformPassenger(this);

                MovementManager.SetPosition(position, false);
                MovementManager.SetRotation(rotation, false);
            }
        }

        /// <summary>
        /// Add <see cref="IWorldEntity"/> as a passenger on this <see cref="IWorldEntity"/>.
        /// </summary>
        public void AddPlatformPassenger(IWorldEntity passenger)
        {
            platformPassengerGuids.Add(passenger.Guid);
        }

        /// <summary>
        /// Remove <see cref="IWorldEntity"/> as a passenger on this <see cref="IWorldEntity"/>.
        /// </summary>
        public void RemovePlatformPassenger(IWorldEntity passenger)
        {
            platformPassengerGuids.Remove(passenger.Guid);
        }
    }
}
