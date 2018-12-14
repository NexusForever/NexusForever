using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
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

        public Inventory Inventory { get; }
        public CurrencyManager CurrencyManager { get; }
        public WorldSession Session { get; }

        private double timeToSave = SaveDuration;
        private PlayerSaveMask saveMask;

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

            Inventory   = new Inventory(this, model);
            Session     = session;

            // temp
            Properties.Add(Property.BaseHealth, new PropertyValue(Property.BaseHealth, 200f, 800f));
            Properties.Add(Property.MoveSpeedMultiplier, new PropertyValue(Property.MoveSpeedMultiplier, 1f, 1f));
            Properties.Add(Property.JumpHeight, new PropertyValue(Property.JumpHeight, 2.5f, 2.5f));
            Properties.Add(Property.GravityMultiplier, new PropertyValue(Property.GravityMultiplier, 1f, 1f));

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
        }

        public override void Update(double lastTick)
        {
            timeToSave -= lastTick;
            if (timeToSave <= 0d)
            {
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
                    FactionId = 166,
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
            /*Session.EnqueueMessageEncrypted(new ServerEntityDestory
            {
                Guid = entity.Guid,
                Unknown0 = true,
                Unknown1 = 2
            });*/
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

                saveMask = PlayerSaveMask.None;
            }
            Inventory.Save(context);
            CurrencyManager.Save(context);
        }
    }
}
