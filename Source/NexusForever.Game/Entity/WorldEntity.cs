using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.CSI;
using NexusForever.Game.Entity.Movement;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Reputation;
using NexusForever.Game.Social;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Social;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NetworkPropertyValue = NexusForever.Network.World.Message.Model.Shared.PropertyValue;

namespace NexusForever.Game.Entity
{
    public abstract class WorldEntity : GridEntity, IWorldEntity
    {
        public EntityType Type { get; }
        public EntityCreateFlag CreateFlags { get; set; }
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Dictionary<Property, IPropertyValue> Properties { get; } = new();

        public uint EntityId { get; protected set; }

        public uint CreatureId
        {
            get => CreatureEntry?.Id ?? 0;
            private set
            {
                CreatureEntry = GameTableManager.Instance.Creature2.GetEntry(value);
            }
        }

        public Creature2Entry CreatureEntry { get; private set; }

        public uint DisplayInfo
        {
            get => CreatureDisplayEntry?.Id ?? 0;
            set
            {
                CreatureDisplayEntry = GameTableManager.Instance.Creature2DisplayInfo.GetEntry(value);
            }
        }

        public Creature2DisplayInfoEntry CreatureDisplayEntry { get; private set; }

        public ushort OutfitInfo
        {
            get => (ushort)(CreatureOutfitEntry?.Id ?? 0);
            private set
            {
                CreatureOutfitEntry = GameTableManager.Instance.Creature2OutfitInfo.GetEntry(value);
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

        public uint MaxHealth
        {
            get => (uint)(GetPropertyValue(Property.BaseHealth) ?? 0u);
        }

        public uint Health
        {
            get => GetStatInteger(Stat.Health) ?? 0u;
        }

        public float Shield
        {
            get => GetStatInteger(Stat.Shield) ?? 0u;
        }

        public uint Level
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

        protected readonly Dictionary<Stat, IStatValue> stats = new();

        private readonly Dictionary<ItemSlot, ItemVisual> itemVisuals = new();

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
        }

        /// <summary>
        /// Initialise <see cref="IWorldEntity"/> from an existing database model.
        /// </summary>
        public virtual void Initialise(EntityModel model)
        {
            EntityId     = model.Id;
            CreatureId   = model.Creature;
            Rotation     = new Vector3(model.Rx, model.Ry, model.Rz);
            DisplayInfo  = model.DisplayInfo;
            OutfitInfo   = model.OutfitInfo;
            Faction1     = (Faction)model.Faction1;
            Faction2     = (Faction)model.Faction2;
            ActivePropId = model.ActivePropId;
            WorldSocketId = model.WorldSocketId;

            foreach (EntityStatModel statModel in model.EntityStat)
                stats.Add((Stat)statModel.Stat, new StatValue(statModel));
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
        }

        protected abstract IEntityModel BuildEntityModel();

        public virtual ServerEntityCreate BuildCreatePacket()
        {
            var entityCreatePacket = new ServerEntityCreate
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
                VisibleItems = itemVisuals.Values.ToList(),
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
            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(CreatureId) ?? Enumerable.Empty<uint>())
                activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate

            // TODO: Fire Scripts
        }

        /// <summary>
        /// Invoked when <see cref="IWorldEntity"/>'s activation fails.
        /// </summary>
        public virtual void OnActivateFail(IPlayer activator)
        {
            // TODO: Fire Scripts
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
        /// Update the display info for the <see cref="IWorldEntity"/>, this overrides any other appearance changes.
        /// </summary>
        public void SetDisplayInfo(uint displayInfo)
        {
            DisplayInfo = displayInfo;

            EnqueueToVisible(new ServerEntityVisualUpdate
            {
                UnitId      = Guid,
                DisplayInfo = DisplayInfo
            }, true);
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
