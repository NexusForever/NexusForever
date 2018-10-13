using NexusForever.Shared.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Player : WorldEntity
    {
        public WorldSession Session { get; }
        public Character Character { get; }

        private PendingFarTeleport pendingFarTeleport;

        public Player(WorldSession session, Character character)
            : base(EntityType.Player)
        {
            Session = session;
            Character = character;

            // temp
            Stats.Add(Stat.Health, new StatValue(Stat.Health, 800));

            // temp
            Properties.Add(Property.BaseHealth, new PropertyValue(Property.BaseHealth, 200f, 800f));
            Properties.Add(Property.MoveSpeedMultiplier, new PropertyValue(Property.MoveSpeedMultiplier, 1f, 1f));
            Properties.Add(Property.JumpHeight, new PropertyValue(Property.JumpHeight, 2.5f, 2.5f));
            Properties.Add(Property.GravityMultiplier, new PropertyValue(Property.GravityMultiplier, 1f, 1f));

            foreach (CharacterAppearance appearance in Character.CharacterAppearance)
            {
                ItemSlot slot = (ItemSlot)appearance.Slot;
                VisibleItems.Add(slot, new VisibleItem(slot, appearance.DisplayId));
            }

            VisibleItems.Add(ItemSlot.WeaponPrimary, new VisibleItem(ItemSlot.WeaponPrimary, 7815));
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PlayerEntityModel
            {
                Id = Character.Id,
                Unknown8 = 358,
                Name = Character.Name,
                Race = (Race)Character.Race,
                Class = (Class)Character.Class,
                Sex = (Sex)Character.Sex
            };
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            Session.EnqueueMessageEncrypted(new ServerChangeWorld
            {
                WorldId = (ushort)map.Entry.Id,
                Position = new Position(vector)
            });

            //TEMP FROM XAN: Set default spawn location somewhere else. Edit vector input?
            //Don't bother with it yet.

            base.OnAddToMap(map, guid, vector);

            Session.EnqueueMessageEncrypted(new ServerPathLog());
            Session.EnqueueMessageEncrypted(new Server00F1());
            Session.EnqueueMessageEncrypted(new Server0636
            {
                Unknown0 = 1,
                Unknown4 = true,
            });

            Session.EnqueueMessageEncrypted(new ServerPlayerEnteredWorld());
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
                    Guid = entity.Guid,
                    Unknown1 = 1
                });
            }

            if (entity is Mount mount && mount.OwnerGuid == Guid)
            {
                Session.EnqueueMessageEncrypted(new Server08B3
                {
                    MountGuid = mount.Guid,
                    Unknown0 = 0,
                    Unknown1 = true
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
                    Faction = 166
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
                //To do still.
            }

            pendingFarTeleport = new PendingFarTeleport(worldId, x, y, z);
            RemoveFromMap();
        }
    }
}
