using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Player : UnitEntity, ISaveCharacter
    {
        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        public ulong CharacterId { get; }
        public string Name { get; }
        public Sex Sex { get; }
        public Race Race { get; }
        public Class Class { get; }
        public List<float> Bones { get; } = new List<float>();

        public byte Level
        {
            get => level;
            set
            {
                level = value;
                saveMask |= PlayerSaveMask.Level;
            }
        }

        private byte level;

        public Path Path
        {
            get => path;
            set
            {
                path = value;
                saveMask |= PlayerSaveMask.Path;
            }
        }
        private Path path;

        public WorldSession Session { get; }
        public bool IsLoading { get; private set; } = true;

        public Inventory Inventory { get; }
        public CurrencyManager CurrencyManager { get; }
        public PathManager PathManager { get; }
        public TitleManager TitleManager { get; }
        public SpellManager SpellManager { get; }

        public VendorInfo SelectedVendorInfo { get; set; } // TODO unset this when too far away from vendor

        private double timeToSave = SaveDuration;
        private PlayerSaveMask saveMask;

        private LogoutManager logoutManager;
        private PendingTeleport pendingTeleport;

        public Player(WorldSession session, Character model)
            : base(EntityType.Player)
        {
            Session         = session;

            CharacterId     = model.Id;
            Name            = model.Name;
            Sex             = (Sex)model.Sex;
            Race            = (Race)model.Race;
            Class           = (Class)model.Class;
            Level           = model.Level;
            Path            = (Path)model.ActivePath;
            Faction1        = (Faction)model.FactionId;
            Faction2        = (Faction)model.FactionId;

            // managers
            Inventory       = new Inventory(this, model);
            CurrencyManager = new CurrencyManager(this, model);
            PathManager     = new PathManager(this, model);
            TitleManager    = new TitleManager(this, model);
            SpellManager    = new SpellManager(this, model);

            Stats.Add(Stat.Level, new StatValue(Stat.Level, level));

            // temp
            Stats.Add(Stat.Health, new StatValue(Stat.Health, 800));

            // temp
            Properties.Add(Property.BaseHealth, new PropertyValue(Property.BaseHealth, 200f, 800f));
            Properties.Add(Property.MoveSpeedMultiplier, new PropertyValue(Property.MoveSpeedMultiplier, 1f, 1f));
            Properties.Add(Property.JumpHeight, new PropertyValue(Property.JumpHeight, 2.5f, 2.5f));
            Properties.Add(Property.GravityMultiplier, new PropertyValue(Property.GravityMultiplier, 1f, 1f));

            // temp
            // TODO:
            // a) move (Add's) to CharacterHandler / CharacterCration
            // b) store abilities persistently
            // c) handle starting abilities by class - sadly no tbl data available...
            SpellManager.AddSpell(47769); // Transmat to Illium
            SpellManager.AddSpell(38934); // some pewpew mount
            SpellManager.AddSpell(62503); // falkron mount
            SpellManager.AddSpell(63431); // zBoard 79 mount
            SpellManager.AddSpell(31213); // Spellsurge
            SpellManager.AddSpell(38229); // Portal capital city
            SpellManager.AddSpell(23148); // Shred
            SpellManager.AddSpell(23161); // Impale
            SpellManager.AddSpell(23173); // Stagger
            SpellManager.AddSpell(46803); // Summon Group
            SpellManager.AddSpellToActionSet(0, 23148, UILocation.LAS1);
            SpellManager.AddSpellToActionSet(0, 23161, UILocation.LAS2, 2);
            SpellManager.AddSpellToActionSet(0, 23173, UILocation.LAS3, 3);
            SpellManager.AddSpellToActionSet(0, 46803, UILocation.PathAbility);

            foreach (ItemVisual itemVisual in Inventory.GetItemVisuals())
                itemVisuals.Add(itemVisual.Slot, itemVisual);

            // character appearance is also 'equipped'
            foreach (CharacterAppearance appearance in model.CharacterAppearance)
            {
                ItemSlot itemSlot = (ItemSlot)appearance.Slot;
                itemVisuals.Add(itemSlot, new ItemVisual
                {
                    Slot      = itemSlot,
                    DisplayId = appearance.DisplayId
                });
            }

            foreach(CharacterBone bone in model.CharacterBone.OrderBy(bone => bone.BoneIndex))
                Bones.Add(bone.Bone);
        }

        public override void Update(double lastTick)
        {
            if (logoutManager != null)
            {
                // don't process world updates while logout is finalising
                if (logoutManager.ReadyToLogout)
                    return;

                logoutManager.Update(lastTick);
            }
            
            TitleManager.Update(lastTick);
            SpellManager.Update(lastTick);

            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
                timeToSave = SaveDuration;

                Session.EnqueueEvent(new TaskEvent(CharacterDatabase.SavePlayer(this),
                    () =>
                {
                    Session.CanProcessPackets = true;
                    timeToSave = SaveDuration;
                }));

                // prevent packets from being processed until asynchronous player save task is complete
                Session.CanProcessPackets = false;
            }

            base.Update(lastTick);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlayerEntityModel
            {
                Id       = CharacterId,
                RealmId  = WorldServer.RealmId,
                Name     = Name,
                Race     = Race,
                Class    = Class,
                Sex      = Sex,
                Bones    = Bones,
                Title    = TitleManager.ActiveTitleId,
                PvPFlag  = PvPFlag.Disabled
            };
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            IsLoading = true;

            Session.EnqueueMessageEncrypted(new ServerChangeWorld
            {
                WorldId  = (ushort)map.Entry.Id,
                Position = new Position(vector)
            });

            base.OnAddToMap(map, guid, vector);

            SendPacketsAfterAddToMap();

            Session.EnqueueMessageEncrypted(new ServerPlayerEnteredWorld());

            IsLoading = false;
        }

        public override void OnRelocate(Vector3 vector)
        {
            base.OnRelocate(vector);
            saveMask |= PlayerSaveMask.Location;
        }

        private void SendPacketsAfterAddToMap()
        {
            PathManager.SendPathLogPacket();
            BuybackManager.SendBuybackItems(this);

            Session.EnqueueMessageEncrypted(new ServerHousingNeighbors());
            
            Session.EnqueueMessageEncrypted(new ServerPathLog());
            Session.EnqueueMessageEncrypted(new Server00F1());
            Session.EnqueueMessageEncrypted(new ServerMovementControl
            {
                Ticket = 1,
                Immediate = true,
            });

            var playerCreate = new ServerPlayerCreate
            {
                ItemProficiencies = GetItemProficiences(),
                FactionData       = new ServerPlayerCreate.Faction
                {
                    FactionId = Faction1, // This does not do anything for the player's "main" faction. Exiles/Dominion
                }
            };

            for (uint i = 1u; i < 17u; i++)
            {
                Currency currency = CurrencyManager.GetCurrency(i);
                if (currency != null)
                    playerCreate.Money[i - 1] = currency.Amount;
            }

            foreach (Item item in Inventory
                .Where(b => b.Location != InventoryLocation.Ability)
                .SelectMany(i => i))
            {
                playerCreate.Inventory.Add(new InventoryItem
                {
                    Item   = item.BuildNetworkItem(),
                    Reason = 49
                });
            }

            Session.EnqueueMessageEncrypted(playerCreate);

            TitleManager.SendTitles();
            SpellManager.SendInitialPackets();
        }

        public ItemProficiency GetItemProficiences()
        {
            ClassEntry classEntry = GameTableManager.Class.GetEntry((ulong)Class);
            return (ItemProficiency)classEntry.StartingItemProficiencies;

            //TODO: Store proficiences in DB table and load from there. Do they change ever after creation? Perhaps something for use on custom servers?
        }

        public override void OnRemoveFromMap()
        {
            base.OnRemoveFromMap();

            if (pendingTeleport != null)
            {
                MapManager.AddToMap(this, pendingTeleport.Info, pendingTeleport.Vector);
                pendingTeleport = null;
            }
        }

        public override void AddVisible(GridEntity entity)
        {
            base.AddVisible(entity);
            Session.EnqueueMessageEncrypted(((WorldEntity)entity).BuildCreatePacket());

            if (entity is Player player)
                player.PathManager.SendSetUnitPathTypePacket();

            if (entity == this)
            {
                Session.EnqueueMessageEncrypted(new ServerPlayerChanged
                {
                    Guid     = entity.Guid,
                    Unknown1 = 1
                });
            }

            if (entity is Mount mount && mount.OwnerGuid == Guid)
            {
                Session.EnqueueMessageEncrypted(new Server08B3
                {
                    MountGuid = mount.Guid,
                    Unknown0  = 0,
                    Unknown1  = true
                });

                // sets mount nameplate to show owner instead of creatures
                // handler calls Mount LUA event
                Session.EnqueueMessageEncrypted(new Server086F
                {
                    MountGuid = mount.Guid,
                    OwnerGuid = Guid
                });

                Session.EnqueueMessageEncrypted(new Server0934
                {
                    MountGuid = mount.Guid,
                    Faction   = 166
                });
            }
        }

        public override void RemoveVisible(GridEntity entity)
        {
            base.RemoveVisible(entity);
            Session.EnqueueMessageEncrypted(new ServerEntityDestory
            {
                Guid     = entity.Guid,
                Unknown0 = true
            });
        }

        /// <summary>
        /// Start delayed logout with optional supplied time and <see cref="LogoutReason"/>.
        /// </summary>
        public void LogoutStart(double timeToLogout = 30d, LogoutReason reason = LogoutReason.None, bool requested = true)
        {
            if (logoutManager != null)
                return;

            logoutManager = new LogoutManager(timeToLogout, reason, requested);

            Session.EnqueueMessageEncrypted(new ServerCharacterLogoutStart
            {
                TimeTillLogout = (uint)timeToLogout * 1000
            });
        }

        /// <summary>
        /// Cancel the current logout, this will fail if the timer has already elapsed.
        /// </summary>
        public void LogoutCancel()
        {
            // can't cancel logout if timer has already elapsed
            if (logoutManager?.ReadyToLogout ?? false)
                return;

            logoutManager = null;
        }

        /// <summary>
        /// Finishes the current logout, saving and cleaning up the <see cref="Player"/> before redirect to the character screen.
        /// </summary>
        public void LogoutFinish()
        {
            if (logoutManager == null)
                throw new InvalidPacketValueException();

            Session.EnqueueEvent(new TaskEvent(CharacterDatabase.SavePlayer(this),
                () =>
            {
                RemoveFromMap();

                Session.EnqueueMessageEncrypted(new ServerClientLogout
                {
                    Requested = logoutManager.Requested,
                    Reason    = logoutManager.Reason
                });

                Session.Player = null;
            }));
        }

        /// <summary>
        /// Teleport <see cref="Player"/> to supplied location.
        /// </summary>
        public void TeleportTo(ushort worldId, float x, float y, float z, uint instanceId = 0u, ulong residenceId = 0ul)
        {
            WorldEntry entry = GameTableManager.World.GetEntry(worldId);
            if (entry == null)
                throw new ArgumentException();

            TeleportTo(entry, new Vector3(x, y, z), instanceId, residenceId);
        }

        /// <summary>
        /// Teleport <see cref="Player"/> to supplied location.
        /// </summary>
        public void TeleportTo(WorldEntry entry, Vector3 vector, uint instanceId = 0u, ulong residenceId = 0ul)
        {
            if (Map?.Entry.Id == entry.Id)
            {
                // TODO: don't remove player from map if it's the same as destination
            }

            var info = new MapInfo(entry, instanceId, residenceId);
            pendingTeleport = new PendingTeleport(info, vector);
            RemoveFromMap();
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != PlayerSaveMask.None)
            {
                var model = new Character
                {
                    Id = CharacterId
                };

                EntityEntry<Character> entity = context.Attach(model);
                if ((saveMask & PlayerSaveMask.Level) != 0)
                {
                    model.Level = Level;
                    entity.Property(p => p.Level).IsModified = true;
                }

                if ((saveMask & PlayerSaveMask.Location) != 0)
                {
                    model.LocationX = Position.X;
                    entity.Property(p => p.LocationX).IsModified = true;

                    model.LocationY = Position.Y;
                    entity.Property(p => p.LocationY).IsModified = true;

                    model.LocationZ = Position.Z;
                    entity.Property(p => p.LocationZ).IsModified = true;

                    model.WorldId = (ushort)Map.Entry.Id;
                    entity.Property(p => p.WorldId).IsModified = true;
                }

                if((saveMask & PlayerSaveMask.Path) != 0)
                {
                    model.ActivePath = (uint)Path;
                    entity.Property(p => p.ActivePath).IsModified = true;
                }

                saveMask = PlayerSaveMask.None;
            }
            Inventory.Save(context);
            CurrencyManager.Save(context);
            PathManager.Save(context);
            TitleManager.Save(context);
        }
    }
}
