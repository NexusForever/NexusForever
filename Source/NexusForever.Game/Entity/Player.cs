using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Reputation;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Achievement;
using NexusForever.Game.Character;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Reputation;
using NexusForever.Game.Social;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Setting;
using NexusForever.Game.Static.Social;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NexusForever.Script;
using NexusForever.Script.Template;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NLog;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Entity
{
    public class Player : UnitEntity, IPlayer
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IPlayer"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum PlayerSaveMask
        {
            None        = 0x0000,
            Location    = 0x0001,
            Path        = 0x0002,
            Costume     = 0x0004,
            InputKeySet = 0x0008,
            Flags       = 0x0020,
            Innate      = 0x0080,
            Sex         = 0x0100,
            Race        = 0x0200,
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        public IAccount Account { get; }

        public ulong CharacterId { get; }
        public string Name { get; }

        public Sex Sex
        {
            get => sex;
            set
            {
                sex = value;
                saveMask |= PlayerSaveMask.Sex;

                SetVisualEmit(true);
            }
        }

        private Sex sex;

        public Race Race
        {
            get => race;
            set
            {
                race = value;
                saveMask |= PlayerSaveMask.Race;

                SetVisualEmit(true);
            }
        }

        private Race race;

        public Class Class { get; }

        public CharacterFlag Flags
        {
            get => flags;
            set
            {
                flags = value;
                saveMask |= PlayerSaveMask.Flags;
            }
        }
        private CharacterFlag flags;

        public Path Path
        {
            get => path;
            set
            {
                path = value;
                PathActivatedTime = DateTime.UtcNow;
                saveMask |= PlayerSaveMask.Path;
            }
        }
        private Path path;

        public DateTime PathActivatedTime { get; private set; }

        public InputSets InputKeySet
        {
            get => inputKeySet;
            set
            {
                inputKeySet = value;
                saveMask |= PlayerSaveMask.InputKeySet;
            }
        }
        private InputSets inputKeySet;

        public byte InnateIndex
        {
            get => innateIndex;
            set
            {
                innateIndex = value;
                saveMask |= PlayerSaveMask.Innate;
            }
        }
        private byte innateIndex;

        public override uint Level
        {
            get => base.Level;
            set
            {
                base.Level = value;

                CalculateDefaultProperties();
                SetBaseCharacterProperties();
            }
        }

        public DateTime CreateTime { get; }
        public double TimePlayedTotal { get; private set; }
        public double TimePlayedLevel { get; private set; }
        public double TimePlayedSession { get; private set; }

        /// <summary>
        /// Guid of the <see cref="IWorldEntity"/> that currently being controlled by the <see cref="IPlayer"/>.
        /// </summary>
        public uint? ControlGuid { get; private set; }

        /// <summary>
        /// Guid of the <see cref="IVehicle"/> the <see cref="IPlayer"/> is a passenger on.
        /// </summary>
        public uint? VehicleGuid
        {
            get => MovementManager.GetPlatform();
            set => MovementManager.SetPlatform(value);
        }

        /// <summary>
        /// Guid of the <see cref="IVanityPet"/> currently summoned by the <see cref="IPlayer"/>.
        /// </summary>
        public uint? VanityPetGuid { get; set; }

        public bool IsSitting => currentChairGuid != null;
        private uint? currentChairGuid;

        /// <summary>
        /// Whether or not this <see cref="Player"/> is currently in a state after using an emote. Setting this to false will let all nearby entities know that the state has been reset.
        /// </summary>
        public bool IsEmoting
        {
            get => isEmoting;
            set
            {
                if (isEmoting && value == false)
                {
                    isEmoting = false;
                    EnqueueToVisible(new ServerEntityEmote
                    {
                        EmotesId = 0,
                        SourceUnitId = Guid
                    });
                    return;
                }

                isEmoting = value;
            }
        }
        private bool isEmoting;

        /// <summary>
        /// Returns if <see cref="IPlayer"/> has premium signature subscription.
        /// </summary>
        public bool SignatureEnabled => Account.RbacManager.HasPermission(Permission.Signature);

        public IGameSession Session { get; }

        /// <summary>
        /// Returns if <see cref="IPlayer"/>'s client is currently in a loading screen.
        /// </summary>
        public bool IsLoading { get; set; } = true;

        public IInventory Inventory { get; }
        public ICurrencyManager CurrencyManager { get; }
        public IPathManager PathManager { get; }
        public ITitleManager TitleManager { get; }
        public ISpellManager SpellManager { get; }
        public ICostumeManager CostumeManager { get; }
        public IPetCustomisationManager PetCustomisationManager { get; }
        public ICharacterKeybindingManager KeybindingManager { get; }
        public IDatacubeManager DatacubeManager { get; }
        public IMailManager MailManager { get; }
        public IZoneMapManager ZoneMapManager { get; }
        public IQuestManager QuestManager { get; }
        public ICharacterAchievementManager AchievementManager { get; }
        public ISupplySatchelManager SupplySatchelManager { get; }
        public IXpManager XpManager { get; }
        public IReputationManager ReputationManager { get; }
        public IGuildManager GuildManager { get; }
        public IChatManager ChatManager { get; }
        public IResidenceManager ResidenceManager { get; }
        public ICinematicManager CinematicManager { get; }
        public ICharacterEntitlementManager EntitlementManager { get; }
        public ILogoutManager LogoutManager { get; }
        public IAppearanceManager AppearanceManager { get; }
        public IResurrectionManager ResurrectionManager { get; }

        public IVendorInfo SelectedVendorInfo { get; set; } // TODO unset this when too far away from vendor

        private UpdateTimer saveTimer = new(SaveDuration);
        private PlayerSaveMask saveMask;

        private Dictionary<Property, Dictionary<ItemSlot, /*value*/float>> itemProperties = new();

        /// <summary>
        /// Create a new <see cref="IPlayer"/> from supplied <see cref="IGameSession"/> and <see cref="CharacterModel"/>.
        /// </summary>
        public Player(IGameSession session, IAccount account, CharacterModel model)
            : base(EntityType.Player)
        {
            ActivationRange   = BaseMap.DefaultVisionRange;

            Session           = session;

            Account           = account;
            CharacterId       = model.Id;
            Name              = model.Name;
            sex               = (Sex)model.Sex;
            race              = (Race)model.Race;
            Class             = (Class)model.Class;
            path              = (Path)model.ActivePath;
            PathActivatedTime = model.PathActivatedTimestamp;
            InputKeySet       = (InputSets)model.InputKeySet;
            Faction1          = (Faction)model.FactionId;
            Faction2          = (Faction)model.FactionId;
            innateIndex       = model.InnateIndex;
            flags             = (CharacterFlag)model.Flags;

            CreateTime        = model.CreateTime;
            TimePlayedTotal   = model.TimePlayedTotal;
            TimePlayedLevel   = model.TimePlayedLevel;

            foreach (CharacterStatModel statModel in model.Stat)
                stats.Add((Stat)statModel.Stat, new StatValue(statModel));

            SetStat(Stat.Sheathed, 1u);
            // temp
            SetStat(Stat.Dash, 200F);
            // sprint
            SetStat(Stat.Resource0, 500f);

            CalculateDefaultProperties();
            SetBaseCharacterProperties();

            scriptCollection = ScriptManager.Instance.InitialiseEntityScripts<IPlayer>(this);

            // managers
            EntitlementManager      = new CharacterEntitlementManager(this, model);
            Account.RewardPropertyManager.Initialise(this);

            CostumeManager          = new CostumeManager(this, model);
            Inventory               = new Inventory(this, model);
            CurrencyManager         = new CurrencyManager(this, model);
            PathManager             = new PathManager(this, model);
            TitleManager            = new TitleManager(this, model);
            SpellManager            = new SpellManager(this, model);
            PetCustomisationManager = new PetCustomisationManager(this, model);
            KeybindingManager       = new CharacterKeybindingManager(this, model);
            DatacubeManager         = new DatacubeManager(this, model);
            MailManager             = new MailManager(this, model);
            ZoneMapManager          = new ZoneMapManager(this, model);
            QuestManager            = new QuestManager(this, model);
            AchievementManager      = new CharacterAchievementManager(this, model);
            SupplySatchelManager    = new SupplySatchelManager(this, model);
            XpManager               = new XpManager(this, model);
            ReputationManager       = new ReputationManager(this, model);
            GuildManager            = new GuildManager(this, model);
            ChatManager             = new ChatManager(this);
            ResidenceManager        = new ResidenceManager(this);
            CinematicManager        = new CinematicManager(this);

            LogoutManager           = new LogoutManager(this);
            LogoutManager.OnTimerFinished += Logout;

            AppearanceManager       = new AppearanceManager(this, model);
            ResurrectionManager     = new ResurrectionManager(this);

            // do dependant stat balance after all stats and properties have been set
            SetDependantStatBalance(true);
            foreach (IPropertyValue property in GetProperties())
                DependantStatBalance(property);

            PlayerManager.Instance.AddPlayer(this);
        }

        private void SetBaseCharacterProperties()
        {
            var baseProperties  = CharacterManager.Instance.GetCharacterBaseProperties();
            var classProperties = CharacterManager.Instance.GetCharacterClassBaseProperties(Class);

            foreach (IPropertyModifier propertyValue in baseProperties.Concat(classProperties))
                SetBaseProperty(propertyValue.Property, propertyValue.GetValue(Level));
        }

        public override void Update(double lastTick)
        {
             LogoutManager.Update(lastTick);

            // don't process world updates while logout is finalising
            if (LogoutManager.State is LogoutState.Logout or LogoutState.Finished)
                return;

            base.Update(lastTick);

            TitleManager.Update(lastTick);
            SpellManager.Update(lastTick);
            CostumeManager.Update(lastTick);
            QuestManager.Update(lastTick);

            saveTimer.Update(lastTick);
            if (saveTimer.HasElapsed)
            {
                double timeSinceLastSave = GetTimeSinceLastSave();
                TimePlayedSession += timeSinceLastSave;
                TimePlayedLevel += timeSinceLastSave;
                TimePlayedTotal += timeSinceLastSave;

                Save();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            QuestManager.Dispose();
        }

        /// <summary>
        /// Save <see cref="IPlayer"/> to database, invoke supplied <see cref="Action"/> once save is complete.
        /// </summary>
        /// <remarks>
        /// This is a delayed save, <see cref="AuthContext"/> changes are saved first followed by <see cref="CharacterContext"/> changes.
        /// Packets for session will not be handled until save is complete.
        /// </remarks>
        public void Save(Action callback = null)
        {
            Session.Events.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.GetDatabase<AuthDatabase>().Save(Save),
            () =>
            {
                Session.Events.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(Save),
                () =>
                {
                    callback?.Invoke();
                    Session.CanProcessIncomingPackets = true;
                    saveTimer.Resume();
                }));
            }));

            saveTimer.Reset(false);

            // prevent packets from being processed until asynchronous player save task is complete
            Session.CanProcessIncomingPackets = false;
        }

        /// <summary>
        /// Save <see cref="IPlayer"/> to database.
        /// </summary>
        /// <remarks>
        /// This is an instant save, <see cref="AuthContext"/> changes are saved first followed by <see cref="CharacterContext"/> changes.
        /// This will block the calling thread until the database save is complete. 
        /// </remarks>
        public async void SaveDirect()
        {
            await DatabaseManager.Instance.GetDatabase<AuthDatabase>().Save(Save);
            await DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(Save);
        }

        /// <summary>
        /// Save database changes for <see cref="Player"/> to <see cref="AuthContext"/>.
        /// </summary>
        public void Save(AuthContext context)
        {
            Account.Save(context);
        }

        /// <summary>
        /// Save database changes for <see cref="Player"/> to <see cref="CharacterContext"/>.
        /// </summary>
        public void Save(CharacterContext context)
        {
            var model = new CharacterModel
            {
                Id = CharacterId
            };

            EntityEntry<CharacterModel> entity = context.Attach(model);

            if (saveMask != PlayerSaveMask.None)
            {
                if ((saveMask & PlayerSaveMask.Location) != 0)
                {
                    model.LocationX = Position.X;
                    entity.Property(p => p.LocationX).IsModified = true;

                    model.LocationY = Position.Y;
                    entity.Property(p => p.LocationY).IsModified = true;

                    model.LocationZ = Position.Z;
                    entity.Property(p => p.LocationZ).IsModified = true;

                    model.RotationX = Rotation.X;
                    entity.Property(p => p.RotationX).IsModified = true;

                    model.RotationY = Rotation.Y;
                    entity.Property(p => p.RotationY).IsModified = true;

                    model.RotationZ = Rotation.Z;
                    entity.Property(p => p.RotationZ).IsModified = true;

                    model.WorldId = (ushort)Map.Entry.Id;
                    entity.Property(p => p.WorldId).IsModified = true;

                    model.WorldZoneId = (ushort)Zone.Id;
                    entity.Property(p => p.WorldZoneId).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.Path) != 0)
                {
                    model.ActivePath = (uint)Path;
                    entity.Property(p => p.ActivePath).IsModified = true;
                    model.PathActivatedTimestamp = PathActivatedTime;
                    entity.Property(p => p.PathActivatedTimestamp).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.InputKeySet) != 0)
                {
                    model.InputKeySet = (sbyte)InputKeySet;
                    entity.Property(p => p.InputKeySet).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.Flags) != 0)
                {
                    model.Flags = (uint)Flags;
                    entity.Property(p => p.Flags).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.Innate) != 0)
                {
                    model.InnateIndex = InnateIndex;
                    entity.Property(p => p.InnateIndex).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.Sex) != 0)
                {
                    model.Sex = (byte)Sex;
                    entity.Property(p => p.Sex).IsModified = true;                    
                }

                if ((saveMask & PlayerSaveMask.Race) != 0)
                {
                    model.Race = (byte)Race;
                    entity.Property(p => p.Race).IsModified = true;
                }

                saveMask = PlayerSaveMask.None;
            }

            model.TimePlayedLevel = (uint)TimePlayedLevel;
            entity.Property(p => p.TimePlayedLevel).IsModified = true;
            model.TimePlayedTotal = (uint)TimePlayedTotal;
            entity.Property(p => p.TimePlayedTotal).IsModified = true;
            model.LastOnline = DateTime.UtcNow;
            entity.Property(p => p.LastOnline).IsModified = true;

            foreach (IStatValue stat in stats.Values)
                stat.SaveCharacter(CharacterId, context);

            Inventory.Save(context);
            CurrencyManager.Save(context);
            PathManager.Save(context);
            TitleManager.Save(context);
            CostumeManager.Save(context);
            PetCustomisationManager.Save(context);
            KeybindingManager.Save(context);
            SpellManager.Save(context);
            DatacubeManager.Save(context);
            MailManager.Save(context);
            ZoneMapManager.Save(context);
            QuestManager.Save(context);
            AchievementManager.Save(context);
            SupplySatchelManager.Save(context);
            XpManager.Save(context);
            ReputationManager.Save(context);
            GuildManager.Save(context);
            EntitlementManager.Save(context);
            AppearanceManager.Save(context);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlayerEntityModel
            {
                Id        = CharacterId,
                RealmId   = RealmContext.Instance.RealmId,
                Name      = Name,
                Race      = Race,
                Class     = Class,
                Sex       = Sex,
                Bones     = AppearanceManager.GetBones()
                    .Select(b => b.BoneValue)
                    .ToList(),
                Title     = TitleManager.ActiveTitleId,
                GuildIds  = GuildManager
                    .Select(g => g.Id)
                    .ToList(),
                GuildName = GuildManager.GuildAffiliation?.Name,
                GuildType = GuildManager.GuildAffiliation?.Type ?? GuildType.None,
                PvPFlag   = PvPFlag.Disabled
            };
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            IsLoading = true;

            Session.EnqueueMessageEncrypted(new ServerChangeWorld
            {
                WorldId  = (ushort)map.Entry.Id,
                Position = new Position(vector),
                Yaw      = Rotation.X
            });

            // this must come before OnAddToMap
            // the client UI initialises the Holomark checkboxes during OnDocumentReady
            SendCharacterFlagsUpdated();

            base.OnAddToMap(map, guid, vector);

            // resummon vanity pet if it existed before teleport
            if (pendingTeleport?.VanityPetId != null)
            {
                var vanityPet = new VanityPet(this, pendingTeleport.VanityPetId.Value);

                var position = new MapPosition
                {
                    Position = Position
                };

                if (map.CanEnter(vanityPet, position))
                    map.EnqueueAdd(vanityPet, position);
            }

            pendingTeleport = null;

            SendPacketsAfterAddToMap();

            if (!IsAlive)
                OnDeath();

            if (PreviousMap == null)
                OnLogin();
        }

        public override void OnRelocate(Vector3 vector)
        {
            base.OnRelocate(vector);
            saveMask |= PlayerSaveMask.Location;

            ZoneMapManager.OnRelocate(vector);
        }

        protected override void OnZoneUpdate()
        {
            if (Zone != null)
            {
                TextTable tt = GameTableManager.Instance.GetTextTable(Language.English);
                if (tt != null)
                {
                    GlobalChatManager.Instance.SendMessage(Session, $"New Zone: ({Zone.Id}){tt.GetEntry(Zone.LocalizedTextIdName)}");
                }

                uint tutorialId = AssetManager.Instance.GetTutorialIdForZone(Zone.Id);
                if (tutorialId > 0)
                {
                    Session.EnqueueMessageEncrypted(new ServerTutorial
                    {
                        TutorialId = tutorialId
                    });
                }

                QuestManager.ObjectiveUpdate(QuestObjectiveType.EnterZone, Zone.Id, 1);
            }

            ZoneMapManager.OnZoneUpdate();
        }

        private void SendPacketsAfterAddToMap()
        {
            DateTime start = DateTime.UtcNow;

            SendInGameTime();
            PathManager.SendInitialPackets();
            BuybackManager.Instance.SendBuybackItems(this);

            ResidenceManager.SendHousingBasics();
            Session.EnqueueMessageEncrypted(new ServerHousingNeighbors());
            Session.EnqueueMessageEncrypted(new ServerInstanceSettings());

            SetControl(this);

            CostumeManager.SendInitialPackets();
            Account.CostumeManager.SendInitialPackets();

            var playerCreate = new ServerPlayerCreate
            {
                ItemProficiencies = GetItemProficiencies(),
                FactionData       = new ServerPlayerCreate.Faction
                {
                    FactionId          = Faction1, // This does not do anything for the player's "main" faction. Exiles/Dominion
                    FactionReputations = ReputationManager
                        .Select(r => new ServerPlayerCreate.Faction.FactionReputation
                        {
                            FactionId = r.Id,
                            Value     = r.Amount
                        })
                        .ToList()
                },
                ActiveCostumeIndex    = CostumeManager.CostumeIndex ?? -1,
                InputKeySet           = (uint)InputKeySet,
                CharacterEntitlements = EntitlementManager
                    .Select(e => new ServerPlayerCreate.CharacterEntitlement
                    {
                        Entitlement = e.Type,
                        Count       = e.Amount
                    })
                    .ToList(),
                TradeskillMaterials   = SupplySatchelManager.BuildNetworkPacket(),
                Xp                    = XpManager.TotalXp,
                RestBonusXp           = XpManager.RestBonusXp
            };

            foreach (ICurrency currency in CurrencyManager)
                playerCreate.Money[(byte)currency.Id - 1] = currency.Amount;

            foreach (IItem item in Inventory
                .Where(b => b.Location != InventoryLocation.Ability)
                .SelectMany(i => i))
            {
                playerCreate.Inventory.Add(new InventoryItem
                {
                    Item   = item.Build(),
                    Reason = ItemUpdateReason.NoReason
                });
            }

            playerCreate.SpecIndex = SpellManager.ActiveActionSet;
            Session.EnqueueMessageEncrypted(playerCreate);

            TitleManager.SendTitles();
            SpellManager.SendInitialPackets();
            PetCustomisationManager.SendInitialPackets();
            DatacubeManager.SendInitialPackets();
            MailManager.SendInitialPackets();
            ZoneMapManager.SendInitialPackets();
            Account.CurrencyManager.SendInitialPackets();
            QuestManager.SendInitialPackets();
            AchievementManager.SendInitialPackets(null);
            Account.RewardPropertyManager.SendInitialPackets();
            ResurrectionManager.SendInitialPackets();

            Session.EnqueueMessageEncrypted(new ServerPlayerInnate
            {
                InnateIndex = InnateIndex
            });

            log.Trace($"Player {Name} took {(DateTime.UtcNow - start).TotalMilliseconds}ms to send packets after add to map.");
        }

        public ItemProficiency GetItemProficiencies()
        {
            //TODO: Store proficiencies in DB table and load from there. Do they change ever after creation? Perhaps something for use on custom servers?
            ClassEntry classEntry = GameTableManager.Instance.Class.GetEntry((ulong)Class);
            return (ItemProficiency)classEntry.StartingItemProficiencies;
        }

        public override void OnRemoveFromMap()
        {
            DestroyDependents();
            base.OnRemoveFromMap();
        }

        public override void AddVisible(IGridEntity entity)
        {
            base.AddVisible(entity);

            if (entity is IWorldEntity worldEntity)
                Session.EnqueueMessageEncrypted(worldEntity.BuildCreatePacket());

            if (entity is IPlayer playerEntity)
                Session.EnqueueMessageEncrypted(new ServerSetUnitPathType
                {
                    Guid = playerEntity.Guid,
                    Path = playerEntity.Path
                });

            if (entity == this)
            {
                Session.EnqueueMessageEncrypted(new ServerPlayerChanged
                {
                    Guid = entity.Guid,
                    Unknown1 = 1
                });
            }

            if (entity is IUnitEntity unitEntity && unitEntity.InCombat)
            {
                Session.EnqueueMessageEncrypted(new ServerUnitEnteredCombat
                {
                    UnitId = unitEntity.Guid,
                    InCombat = unitEntity.InCombat
                });
            }
        }

        public override void RemoveVisible(IGridEntity entity)
        {
            base.RemoveVisible(entity);

            if (entity is IWorldEntity && entity != this)
            {
                Session.EnqueueMessageEncrypted(new ServerEntityDestroy
                {
                    Guid     = entity.Guid,
                    Unknown0 = true
                });
            }
        }

        protected override void AddVisible(uint gridX, uint gridZ)
        {
            base.AddVisible(gridX, gridZ);
            Map.GridAddVisiblePlayer(gridX, gridZ);
        }

        protected override void RemoveVisible(uint gridX, uint gridZ)
        {
            base.RemoveVisible(gridX, gridZ);
            Map.GridRemoveVisiblePlayer(gridX, gridZ);
        }

        /// <summary>
        /// Set the <see cref="IWorldEntity"/> that currently being controlled by the <see cref="IPlayer"/>.
        /// </summary>
        public void SetControl(IWorldEntity entity)
        {
            if (entity.Guid == ControlGuid)
                return;

            if (ControlGuid != null)
            {
                IWorldEntity control = Map.GetEntity<IWorldEntity>(ControlGuid.Value);
                if (control != null)
                    control.ControllerGuid = null;
            }

            uint? guid = entity != this ? entity.Guid : null;
            ControlGuid           = guid;
            entity.ControllerGuid = guid;

            Session.EnqueueMessageEncrypted(new ServerMovementControl
            {
                Ticket    = 1,
                Immediate = true,
                UnitId    = entity.Guid
            });
        }

        private void Logout()
        {
            OnLogout();
            Cleanup();
        }

        private void Cleanup()
        {
            log.Trace($"Cleanup for character {Name}({CharacterId})...");

            PlayerManager.Instance.RemovePlayer(this);
            CleanupManager.Instance.AddPlayer(this);

            log.Trace($"Waiting to cleanup character {Name}({CharacterId})...");

            Session.Events.EnqueueEvent(new TimeoutPredicateEvent(TimeSpan.FromSeconds(15), CanCleanup,
                () =>
            {
                try
                {
                    log.Trace($"Cleanup for character {Name}({CharacterId}) has started...");

                    Save(() =>
                    {
                        if (Map != null)
                            RemoveFromMap();

                        Dispose();
                    });
                }
                finally
                {
                    CleanupManager.Instance.RemovePlayer(this);
                    log.Trace($"Cleanup for character {Name}({CharacterId}) has completed.");

                    LogoutManager.State = LogoutState.Finished;
                }
            }));
        }

        private bool CanCleanup()
        {
            return pendingTeleport == null;
        }

        private void OnLogin()
        {
            scriptCollection.Invoke<IPlayerScript>(s => s.OnLogin());

            string motd = RealmContext.Instance.Motd;
            if (motd?.Length > 0)
                GlobalChatManager.Instance.SendMessage(Session, motd, "MOTD", ChatChannelType.Realm);

            GuildManager.OnLogin();
            ChatManager.OnLogin();
            GlobalChatManager.Instance.JoinDefaultChatChannels(this);

            ShutdownManager.Instance.OnLogin(this);
        }

        private void OnLogout()
        {
            GuildManager.OnLogout();
            ChatManager.OnLogout();
            GlobalChatManager.Instance.LeaveDefaultChatChannels(this);

            scriptCollection.Invoke<IPlayerScript>(s => s.OnLogout());
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can teleport.
        /// </summary>
        public bool CanTeleport() => pendingTeleport == null;
        private PendingTeleport pendingTeleport;

        /// <summary>
        /// Teleport <see cref="IPlayer"/> to supplied location.
        /// </summary>
        public void TeleportTo(ushort worldId, float x, float y, float z, ulong? instanceId = null, TeleportReason reason = TeleportReason.Relocate)
        {
            WorldEntry entry = GameTableManager.Instance.World.GetEntry(worldId);
            if (entry == null)
                throw new ArgumentException($"{worldId} is not a valid world id!");

            TeleportTo(entry, new Vector3(x, y, z), instanceId, reason);
        }

        /// <summary>
        /// Teleport <see cref="IPlayer"/> to supplied location.
        /// </summary>
        public void TeleportTo(WorldEntry entry, Vector3 position, ulong? instanceId = null, TeleportReason reason = TeleportReason.Relocate)
        {
            TeleportTo(new MapPosition
            {
                Info = new MapInfo
                {
                    Entry      = entry,
                    InstanceId = instanceId
                },
                Position = position
            }, reason);
        }

        /// <summary>
        /// Teleport <see cref="IPlayer"/> to supplied location.
        /// </summary>
        public void TeleportTo(IMapPosition mapPosition, TeleportReason reason = TeleportReason.Relocate)
        {
            if (!CanTeleport())
            {
                SendGenericError(GenericError.InstanceTransferPending);
                return;
            }

            if (DisableManager.Instance.IsDisabled(DisableType.World, mapPosition.Info.Entry.Id))
            {
                SendSystemMessage($"Unable to teleport to world {mapPosition.Info.Entry.Id} because it is disabled.");
                return;
            }

            // store vanity pet summoned before teleport so it can be summoned again after being added to the new map
            uint? vanityPetId = null;
            if (VanityPetGuid != null)
            {
                IVanityPet pet = GetVisible<IVanityPet>(VanityPetGuid.Value);
                vanityPetId = pet?.CreatureId;
            }

            pendingTeleport = new PendingTeleport
            {
                Reason      = reason,
                MapPosition = mapPosition,
                VanityPetId = vanityPetId
            };

            MapManager.Instance.AddToMap(this, mapPosition);
            log.Trace($"Teleporting {Name}({CharacterId}) to map: {mapPosition.Info.Entry.Id}, instance: {mapPosition.Info.InstanceId ?? 0ul}.");
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> teleport fails.
        /// </summary>
        public void OnTeleportToFailed(GenericError error)
        {
            if (Map != null)
            {
                SendGenericError(error);
                pendingTeleport = null;

                log.Trace($"Error {error} occured during teleport for {Name}({CharacterId})!");
            }
            else
            {
                // player failed prerequisites to enter map on login
                // can not proceed, disconnect the client with a message
                Session.EnqueueMessageEncrypted(new ServerForceKick
                {
                    Reason = ForceKickReason.WorldDisconnect
                });

                log.Trace($"Error {error} occured during teleport for {Name}({CharacterId}), client will be disconnected!");
            }
        }

        /// <summary>
        /// Used to send the current in game time to this player
        /// </summary>
        private void SendInGameTime()
        {
            uint lengthOfInGameDayInSeconds = SharedConfiguration.Instance.Get<RealmConfig>().LengthOfInGameDay;
            if (lengthOfInGameDayInSeconds == 0u)
                lengthOfInGameDayInSeconds = (uint)TimeSpan.FromHours(3.5d).TotalSeconds; // Live servers were 3.5h per in game day

            double timeOfDay = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds / lengthOfInGameDayInSeconds % 1;

            Session.EnqueueMessageEncrypted(new ServerTimeOfDay
            {
                TimeOfDay   = (uint)(timeOfDay * TimeSpan.FromDays(1).TotalSeconds),
                LengthOfDay = lengthOfInGameDayInSeconds
            });
        }

        /// <summary>
        /// Make <see cref="IPlayer"/> sit on provided <see cref="IWorldEntity"/>.
        /// </summary>
        public void Sit(IWorldEntity chair)
        {
            if (IsSitting)
                Unsit();

            currentChairGuid = chair.Guid;

            // TODO: Emit interactive state from the entity instance itself
            chair.EnqueueToVisible(new ServerEntityInteractiveUpdate
            {
                UnitId = chair.Guid,
                InUse  = true
            }, true);
            EnqueueToVisible(new ServerUnitSetChair
            {
                UnitId      = Guid,
                UnitIdChair = chair.Guid,
                WaitForUnit = false
            }, true);
        }

        /// <summary>
        /// Remove <see cref="IPlayer"/> from the <see cref="IWorldEntity"/> it is sitting on.
        /// </summary>
        public void Unsit()
        {
            if (!IsSitting)
                return;

            IWorldEntity currentChair = GetVisible<IWorldEntity>(currentChairGuid.Value);
            if (currentChair == null)
                throw new InvalidOperationException();

            // TODO: Emit interactive state from the entity instance itself
            currentChair.EnqueueToVisible(new ServerEntityInteractiveUpdate
            {
                UnitId = currentChair.Guid,
                InUse  = false
            }, true);
            EnqueueToVisible(new ServerUnitSetChair
            {
                UnitId      = Guid,
                UnitIdChair = 0,
                WaitForUnit = false
            }, true);

            currentChairGuid = null;
        }

        /// <summary>
        /// Send <see cref="GenericError"/> to <see cref="IPlayer"/>.
        /// </summary>
        public void SendGenericError(GenericError error)
        {
            Session.EnqueueMessageEncrypted(new ServerGenericError
            {
                Error = error
            });
        }

        /// <summary>
        /// Send message to <see cref="IPlayer"/> using the <see cref="ChatChannel.System"/> channel.
        /// </summary>
        /// <param name="text"></param>
        public void SendSystemMessage(string text)
        {
            Session.EnqueueMessageEncrypted(new ServerChat
            {
                Channel = new Channel
                {
                    Type = ChatChannelType.System
                },
                Text = text
            });
        }

        /// <summary>
        /// Returns whether this <see cref="IPlayer"/> is allowed to summon or be added to a mount.
        /// </summary>
        public bool CanMount()
        {
            return VehicleGuid == null && pendingTeleport == null;
        }

        /// <summary>
        /// Dismounts this <see cref="IPlayer"/> from a vehicle that it's attached to
        /// </summary>
        public void Dismount()
        {
            if (VehicleGuid == null)
                return;

            IVehicle vehicle = GetVisible<IVehicle>(VehicleGuid.Value);
            vehicle.PassengerRemove(this);
        }

        /// <summary>
        /// Remove all entities associated with the <see cref="IPlayer"/>
        /// </summary>
        private void DestroyDependents()
        {
            // vehicle will be removed if player is the last passenger
            Dismount();

            if (VanityPetGuid != null)
            {
                IVanityPet pet = GetVisible<IVanityPet>(VanityPetGuid.Value);
                pet?.RemoveFromMap();
                VanityPetGuid = null;
            }

            RemoveControlUnit();

            // TODO: Remove pets, scanbots
        }

        private void RemoveControlUnit()
        {
            if (ControlGuid == null)
                return;

            IWorldEntity controlled = Map.GetEntity<IWorldEntity>(ControlGuid.Value);
            controlled?.RemoveFromMap();
            SetControl(this);
        }

        /// <summary>
        /// Returns the time in seconds that has past since the last <see cref="IPlayer"/> save.
        /// </summary>
        public double GetTimeSinceLastSave()
        {
            return SaveDuration - saveTimer.Time;
        }

        /// <summary>
        /// Return <see cref="Disposition"/> between <see cref="IPlayer"/> and <see cref="Faction"/>.
        /// </summary>
        public override Disposition GetDispositionTo(Faction factionId, bool primary = true)
        {
            IFactionNode targetFaction = FactionManager.Instance.GetFaction(factionId);
            if (targetFaction == null)
                throw new ArgumentException($"Invalid faction {factionId}!");

            // find disposition based on reputation level
            Disposition? dispositionFromReputation = GetDispositionFromReputation(targetFaction);
            if (dispositionFromReputation.HasValue)
                return dispositionFromReputation.Value;

            return base.GetDispositionTo(factionId, primary);
        }

        private Disposition? GetDispositionFromReputation(IFactionNode node)
        {
            if (node == null)
                return null;

            // check if current node has required reputation
            IReputation reputation = ReputationManager.GetReputation(node.FactionId);
            if (reputation != null)
                return FactionNode.GetDisposition(FactionNode.GetFactionLevel(reputation.Amount));

            // check if parent node has required reputation
            return GetDispositionFromReputation(node.Parent);
        }

        /// <summary>
        /// Add a new <see cref="CharacterFlag"/>.
        /// </summary>
        public void SetFlag(CharacterFlag flag)
        {
            Flags |= flag;
            SendCharacterFlagsUpdated();
        }

        /// <summary>
        /// Remove an existing <see cref="CharacterFlag"/>.
        /// </summary>
        public void RemoveFlag(CharacterFlag flag)
        {
            Flags &= ~flag;
            SendCharacterFlagsUpdated();
        }

        /// <summary>
        /// Returns if supplied <see cref="CharacterFlag"/> exists.
        /// </summary>
        public bool HasFlag(CharacterFlag flag)
        {
            return (Flags & flag) != 0;
        }

        /// <summary>
        /// Send <see cref="ServerCharacterFlagsUpdated"/> to client.
        /// </summary>
        public void SendCharacterFlagsUpdated()
        {
            Session.EnqueueMessageEncrypted(new ServerCharacterFlagsUpdated
            {
                Flags = flags
            });
        }

        /// <summary>
        /// Add or update <see cref="IItemVisual"/>.
        /// </summary>
        /// <remarks>
        /// Checks for <see cref="IItemVisual"/> from <see cref="ICostume"/> if equipped for the same <see cref="ItemSlot"/> and replace if necessary.
        /// </remarks>
        public override void AddVisual(IItemVisual visual)
        {
            if (CostumeManager.CostumeIndex.HasValue)
            {
                IItemVisual costumeVisual = CostumeManager.GetItemVisual(CostumeManager.CostumeIndex.Value, visual.Slot);
                if (costumeVisual != null && costumeVisual.DisplayId.HasValue)
                    visual = costumeVisual;
            }

            base.AddVisual(visual);
        }

        protected override ServerEntityVisualUpdate BuildVisualUpdate()
        {
            // TODO: note sure if this should be handled better? Should Race and Sex be on the world entity?
            // when emitting a visual update, include player specific properties
            ServerEntityVisualUpdate update = base.BuildVisualUpdate();
            update.Race = (byte)Race;
            update.Sex  = (byte)Sex;
            return update;
        }

        /// <summary>
        /// Add a <see cref="Property"/> modifier given a <see cref="ItemSlot"/> and value.
        /// </summary>
        public void AddItemProperty(Property property, ItemSlot itemSlot, float value)
        {
            if (itemProperties.TryGetValue(property, out Dictionary<ItemSlot, float> itemDict))
            {
                if (itemDict.ContainsKey(itemSlot))
                    itemDict[itemSlot] = value;
                else
                    itemDict.Add(itemSlot, value);
            }
            else
            {
                itemProperties.Add(property, new Dictionary<ItemSlot, float>
                {
                    { itemSlot, value }
                });
            }

            CalculateProperty(property);
        }

        /// <summary>
        /// Remove a <see cref="Property"/> modifier by a item that is currently affecting this <see cref="IPlayer"/>.
        /// </summary>
        public void RemoveItemProperty(Property property, ItemSlot itemSlot)
        {
            if (itemProperties.TryGetValue(property, out Dictionary<ItemSlot, float> itemDict))
                itemDict.Remove(itemSlot);

            CalculateProperty(property);
        }

        /// <summary>
        /// Calculate the primary value for <see cref="Property"/>.
        /// </summary>
        protected override void CalculatePropertyValue(IPropertyValue propertyValue)
        {
            base.CalculatePropertyValue(propertyValue);

            if (itemProperties.TryGetValue(propertyValue.Property, out Dictionary<ItemSlot, float> properties))
                foreach (float values in properties.Values)
                    propertyValue.Value += values;
        }

        /// <summary>
        /// Determine if this <see cref="IPlayer"/> can attack supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public override bool CanAttack(IUnitEntity target)
        {
            // TODO: Disable when PvP is available.
            if (target is IPlayer)
                return false;

            return base.CanAttack(target);
        }

        /// <summary>
        /// Modify the health of this <see cref="IPlayer"/> by the supplied amount.
        /// </summary>
        /// <remarks>
        /// If the <see cref="DamageType"/> is <see cref="DamageType.Heal"/> amount is added to current health otherwise subtracted.
        /// </remarks>
        public override void ModifyHealth(uint amount, DamageType type, IUnitEntity source)
        {
            base.ModifyHealth(amount, type, source);

            Session.EnqueueMessageEncrypted(new ServerPlayerHealthUpdate
            {
                UnitId = Guid,
                Health = Health,
                Mask   = type switch
                {
                    DamageType.Fall      => UpdateHealthMask.FallDamage,
                    DamageType.Suffocate => UpdateHealthMask.SuffocateDamage,
                    _                    => UpdateHealthMask.None
                }
            });

            if (Health > 0 && DeathState != null)
                OnResurrection(source);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            Dismount();
            RemoveControlUnit();

            // TODO: Replace with DelayEvent (of 2 seconds) with map updates.
            IGhost ghost = new Ghost(this);
            Map.EnqueueAdd(ghost, new MapPosition
            {
                Info = new MapInfo
                {
                    Entry      = Map.Entry,
                    InstanceId = Map is IMapInstance instance ? instance.InstanceId : 0u
                },
                Position = Position
            });
        }

        protected override void RewardKiller(IPlayer player)
        {
            // TODO: handle PvP rewards
        }

        protected void OnResurrection(IUnitEntity resurrector)
        {
            DeathState = null;
            RemoveControlUnit();
        }
    }
}
