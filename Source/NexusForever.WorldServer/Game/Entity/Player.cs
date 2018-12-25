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
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Player : WorldEntity, ISaveCharacter
    {
        // TODO: move this to the config file
        private const double SaveDuration = 60d;

        public ulong CharacterId { get; }
        public string Name { get; }
        public Sex Sex { get; }
        public Race Race { get; }
        public Class Class { get; }
        public List<float> Bones { get; }

        public byte Level { get; }

        public Inventory Inventory { get; }
        public CurrencyManager CurrencyManager { get; }
        public WorldSession Session { get; }

        private double timeToSave = SaveDuration;
        private PlayerSaveMask saveMask;

        private LogoutManager logoutManager;
        private PendingFarTeleport pendingFarTeleport;

        public Player(WorldSession session, Character model)
            : base(EntityType.Player)
        {
            CharacterId = model.Id;
            Name        = model.Name;
            Sex         = (Sex)model.Sex;
            Race        = (Race)model.Race;
            Class       = (Class)model.Class;
            Bones       = new List<float>();
            CurrencyManager = new CurrencyManager(this, model);
            Faction2    = model.FactionId;

            Inventory   = new Inventory(this, model);
            Session     = session;

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
            {
                Bones.Add(bone.Bone);
            }

            foreach(CharacterStat stat in model.CharacterStat)
            {
                if ((StatValue.StatType)stat.Type == StatValue.StatType.Int)
                    Stats.Add((Stat)stat.Stat, new StatValue((Stat)stat.Stat, (uint)stat.Value));
                else if((StatValue.StatType)stat.Type == StatValue.StatType.Float)
                    Stats.Add((Stat)stat.Stat, new StatValue((Stat)stat.Stat, (float)stat.Value));
            }
            Level = (byte)GetStatValue(Stat.Level);

            foreach(CharacterProperty prop in model.CharacterProperty)
            {
                Properties.Add((Property)prop.Property, new PropertyValue((Property)prop.Property, prop.BaseValue, prop.Value));
            }
        }

        public class UnlockAbility
        {
            public uint Spell4BaseId { get; set; }
            public byte Tier {get ; set; } = 1;
            public UILocation Location { get; set; } = UILocation.None;
            //TODO: is this really a "reason" - most Spells are sent as 49 as well, but some are 27/28
            public byte Reason { get; set; } = 49;
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
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlayerEntityModel
            {
                Id       = CharacterId,
                Unknown8 = 358,
                Name     = Name,
                Race     = Race,
                Class    = Class,
                Sex      = Sex,
                Bones    = Bones
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

            ClassEntry classEntry = GameTableManager.Class.GetEntry((ulong)Class);
            Spell4Entry spell4Entry = GameTableManager.Spell4.GetEntry(classEntry.Spell4IdInnateAbilityActive00);

            var unlockAbilities = new List<UnlockAbility>();
            var serverAbilities = new ServerAbilities();
            var serverActionSet = new ServerActionSet{ActionSetIndex = 0};
            byte tierPoints = 42;

            //TODO:
                // a) move (Add's) to CharacterHandler / CharacterCration
                // b) store abilities persistently
                // c) handle starting abilities by class - sadly no tbl data available...
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 47769}); // Transmat to Illium
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 38934}); // some pewpew mount
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 62503}); // falkron mount
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 63431}); // zBoard 79 mount
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 31213}); // Spellsurge
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 38229}); // Portal capital city

            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 23148, Location = UILocation.LAS1});  // Shred
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 23161, Tier = 2, Location = UILocation.LAS2});  // Impale
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 23173, Tier = 3, Location = UILocation.LAS3});  // Stagger
            unlockAbilities.Add(new UnlockAbility{Spell4BaseId = 46803, Location = UILocation.PathAbility});  // Summon Group


            // on AddToMap, the full array is required to be sent - updates can be sent independently
            for (int i = 0; i < 48; i++)
                serverActionSet.actionLocation.Add(new ServerActionSet.ActionLocation {Location = (uint)i});

            foreach(UnlockAbility u in unlockAbilities)
            {
                if (u.Spell4BaseId<1)
                    continue;

                Session.Player.Inventory.SpellCreate(u.Spell4BaseId, u.Reason);
                serverAbilities.ability.Add(new ServerAbilities.Ability {Spell4BaseId = u.Spell4BaseId, Tier = u.Tier});

                int l = (int)u.Location;
                if (u.Spell4BaseId >0 && (UILocation)l != UILocation.None && u.Tier > 0)
                {
                    serverActionSet.actionLocation[l].Unknown0 = 4;
                    serverActionSet.actionLocation[l].Unknown4 = 4;
                    serverActionSet.actionLocation[l].Location =(uint)l;
                    serverActionSet.actionLocation[l].Spell4BaseId = u.Spell4BaseId;
                }
                tierPoints -= (byte)(u.Tier-1);
            }

            Session.EnqueueMessageEncrypted(serverAbilities);

            // TODO: make points stored persistent or always calc?
            Session.EnqueueMessageEncrypted(new ServerAbilityPoints {AbilityPointsAvailable = tierPoints, AbilityPointsSpent = 42}); // enables ability tier points

            Session.EnqueueMessageEncrypted(serverActionSet);
        }

        public override void OnRelocate(Vector3 vector)
        {
            base.OnRelocate(vector);
            saveMask |= PlayerSaveMask.Location;
        }

        private void SendPacketsAfterAddToMap()
        {
            Session.EnqueueMessageEncrypted(new ServerPathLog());
            Session.EnqueueMessageEncrypted(new Server00F1());
            Session.EnqueueMessageEncrypted(new ServerMovementControl
            {
                Ticket = 1,
                Immediate = true,
            });

            var playerCreate = new ServerPlayerCreate
            {
                FactionData = new ServerPlayerCreate.Faction
                {
                    FactionId = 166, // This does not do anything for the player's "main" faction. Exiles/Dominion
                }
            };

            for (uint i = 1u; i < 17u; i++)
            {
                Currency currency = CurrencyManager.GetCurrency(i);
                if (currency != null)
                    playerCreate.Money[i - 1] = currency.Amount;
            }

            foreach (Bag bag in Inventory)
            {
                foreach (Item item in bag)
                {
                    // don't add abilities - client crash
                    if (bag.Location == InventoryLocation.Ability)
                        continue;

                    playerCreate.Inventory.Add(new InventoryItem
                    {
                        Item   = item.BuildNetworkItem(),
                        Reason = 49
                    });
                }
            }

            Session.EnqueueMessageEncrypted(playerCreate);
        }

        public override void OnRemoveFromMap()
        {
            base.OnRemoveFromMap();

            if (pendingFarTeleport != null)
            {
                MapManager.AddToMap(this, pendingFarTeleport.WorldId, pendingFarTeleport.Vector);
                pendingFarTeleport = null;
            }
        }

        public override void AddVisible(GridEntity entity)
        {
            base.AddVisible(entity);
            Session.EnqueueMessageEncrypted(((WorldEntity)entity).BuildCreatePacket());

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
        public void TeleportTo(ushort worldId, float x, float y, float z)
        {
            WorldEntry entry = GameTableManager.World.GetEntry(worldId);
            if (entry == null)
                throw new ArgumentException();

            if (Map != null && Map.Entry.Id == entry.Id)
            {
                // TODO: don't remove player from map if it's the same as destination
            }

            pendingFarTeleport = new PendingFarTeleport(worldId, x, y, z);
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

                saveMask = PlayerSaveMask.None;
            }

            foreach(KeyValuePair<Stat, StatValue> stat in Stats)
            {
                if (stat.Value.IsModified)
                {
                    var model = new CharacterStat
                    {
                        Id = CharacterId,
                        Stat = (byte)stat.Key
                    };
                    EntityEntry<CharacterStat> entity = context.Attach(model);

                    model.Value = stat.Value.Value;
                    entity.Property(p => p.Value).IsModified = true;
                    stat.Value.IsModified = false;
                }
            }

            foreach(KeyValuePair<Property, PropertyValue> property in Properties)
            {
                if (property.Value.IsModified)
                {
                    var model = new CharacterProperty
                    {
                        Id = CharacterId,
                        Property = (byte)property.Key
                    };
                    EntityEntry<CharacterProperty> entity = context.Attach(model);

                    model.Value = property.Value.Value;
                    model.BaseValue = property.Value.BaseValue;
                    entity.Property(p => p.Value).IsModified = true;
                    entity.Property(p => p.BaseValue).IsModified = true;
                    property.Value.IsModified = false;
                }
            }
            Inventory.Save(context);
            CurrencyManager.Save(context);
        }
    }
}
