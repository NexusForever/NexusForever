using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Game.Entity.Network.Command;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Mount : UnitEntity
    {
        public uint OwnerGuid { get; }
        public Creature2Entry Creature { get; }
        public Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; }
        public UnitVehicleEntry UnitVehicle { get; }
        public Spell4Entry Spell { get; }
        public PetType MountType { get; }
        public uint ItemColorSetId { get; set; }

        // 0=??, 1=show spawn animation, 2=do not show spawn animation
        private byte SpawnAnimation { get; set; } = 1;

        public Mount(Player owner, uint creature, uint spell, PetType mountType)
            : base(EntityType.Mount)
        {
            OwnerGuid = owner.Guid;
            Rotation = owner.Rotation;
            Position = owner.Position;
            Spell = GameTableManager.Spell4.GetEntry(spell);
            Creature = GameTableManager.Creature2.GetEntry(creature);
            Creature2DisplayGroup = GameTableManager.Creature2DisplayGroupEntry.Entries.SingleOrDefault(x => x.Creature2DisplayGroupId == Creature.Creature2DisplayGroupId);
            UnitVehicle = GameTableManager.UnitVehicle.GetEntry(Creature.UnitVehicleId); ;
            DisplayInfo = Creature2DisplayGroup.Creature2DisplayInfoId;
            MountType = mountType;

            SetProperty(Property.BaseHealth, 800.0f, 800.0f);

            Stats.Add(Stat.Health, new StatValue(Stat.Health, 800));
            Stats.Add(Stat.Level, new StatValue(Stat.Level, 3));
            Stats.Add((Stat)15, new StatValue((Stat)15, 0));
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new MountEntityModel
            {
                CreatureId = Creature.Id,
                UnitVehicleId = (ushort)UnitVehicle.Id,
                OwnerId = OwnerGuid
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            var entityCreate = base.BuildCreatePacket();
            entityCreate.Unknown60 = SpawnAnimation;
            entityCreate.Unknown68 = 904575;
            entityCreate.Faction1 = Faction1; // 0 on initial spawn
            entityCreate.Faction2 = Faction2; // 0 on initial spawn
            entityCreate.Commands = new Dictionary<EntityCommand, IEntityCommand>
            {
                {
                    EntityCommand.SetPlatform,
                    new SetPlatformCommand
                    {
                        //0
                    }
                },
                {
                    EntityCommand.SetPosition,
                    new SetPositionCommand
                    {
                        Position = new Position(Position)
                    }
                },
                {
                    EntityCommand.SetVelocity,
                    new SetVelocityCommand
                    {
                        //fixme fillme
                    }
                },
                {
                    EntityCommand.SetMove,
                    new SetMoveCommand
                    {
                        //fixme fillme
                    }
                },
                {
                    EntityCommand.SetRotation,
                    new SetRotationCommand
                    {
                        Position = new Position(Rotation)
                    }
                },
                {
                    EntityCommand.SetState,
                    new SetStateCommand
                    {
                        State = 257 // usually 257/258
                    }
                },
                {
                    EntityCommand.SetMode,
                    new SetModeCommand
                    {
                    }
                }
            };
            return entityCreate;
        }

        public override void OnRemoveFromMap()
        {
            base.OnRemoveFromMap();
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);
            MountUp();
        }

        public void UpdateVisuals()
        {
            var owner = Map.GetEntity<Player>(OwnerGuid);

            if (MountType == PetType.GroundMount)
            {
                owner.EnqueueToVisible(new Server0905
                {
                    UnitId = Guid,
                    ItemVisuals = itemVisuals.Values.ToList(),
                    Race = (byte)owner.Race,
                    Sex = (byte)owner.Sex,
                    CreatureId = Creature.Id,
                    DisplayInfo = DisplayInfo,
                    ItemColorSetId = ItemColorSetId
                }, true);
            }
            else
            {
                owner.EnqueueToVisible(new Server0905
                {
                    UnitId = owner.Guid,
                    ItemVisuals = owner.itemVisuals.Values.Concat(itemVisuals.Values).ToList(),
                    Race = (byte)owner.Race,
                    Sex = (byte)owner.Sex,
                    ItemColorSetId = ItemColorSetId
                }, true);
            }
        }

        public void MountUp()
        {
            var owner = Map.GetEntity<Player>(OwnerGuid);

            owner.EnqueueToVisible(new Server08B3
            {
                MountGuid = Guid,
                Unknown0 = 0,
                Unknown1 = true
            }, true);

            owner.EnqueueToVisible(new ServerEntityCommand
            {
                Guid = Guid,
                Time = 1,
                Unknown2 = true,
                Commands = new List<(EntityCommand, IEntityCommand)>
                {
                    (
                        EntityCommand.SetPlatform,
                        new SetPlatformCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetPosition,
                        new SetPositionCommand
                        {
                            Position = new Position(owner.Position), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetVelocityDefaults,
                        new SetVelocityDefaultsCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetMove,
                        new SetMoveCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetRotation,
                        new SetRotationCommand
                        {
                            Position = new Position(owner.Rotation), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetStateDefault,
                        new SetStateDefaultCommand
                        {
                            Strafe = true
                        }
                    ),
                    (
                        EntityCommand.SetModeDefault,
                        new SetModeDefaultCommand
                        {
                        }
                    )
                }
            }, true);

            owner.Session.EnqueueMessageEncrypted(new Server0237
            {
            });

            owner.Session.EnqueueMessageEncrypted(new Server0639
            {
            });

            owner.EnqueueToVisible(new ServerEntityCommand
            {
                Guid = owner.Guid,
                Time = 1,
                Unknown2 = true,
                Commands = new List<(EntityCommand, IEntityCommand)>
                {
                    (
                        EntityCommand.SetPlatform,
                        new SetPlatformCommand
                        {
                            Platform = Guid
                        }
                    ),
                    (
                        EntityCommand.SetPosition,
                        new SetPositionCommand
                        {
                            Position = new Position(new Vector3(0,0,0)), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetRotation,
                        new SetRotationCommand
                        {
                            Position = new Position(new Vector3(0,0,0)), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetState,
                        new SetStateCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetModeDefault,
                        new SetModeDefaultCommand
                        {
                        }
                    )
                }
            }, true);

            // sets mount nameplate to show owner instead of creatures
            // handler calls Mount LUA event
            owner.EnqueueToVisible(new Server086F
            {
                MountGuid = Guid,
                OwnerGuid = owner.Guid
            }, true);

            owner.Session.EnqueueMessageEncrypted(new ServerMovementControl
            {
                Ticket = 1,
                Immediate = true,
                UnitId = Guid
            });

            UpdateVisuals();

            // as on retail, initial entityCreate has faction 0
            owner.EnqueueToVisible(new Server0934
            {
                MountGuid = Guid,
                Faction = (ushort)owner.Faction1
            }, true);
            Faction1 = owner.Faction1;
            Faction2 = owner.Faction2;

            // TODO: what is this packet for?
            List<Server093A.UnknownStructure0> unknownStructure093A = new List<Server093A.UnknownStructure0>();
            unknownStructure093A.Add(new Server093A.UnknownStructure0 { Unknown0 = 100, Unknown1 = 1065353216, Unknown2 = 1065353216 });
            unknownStructure093A.Add(new Server093A.UnknownStructure0 { Unknown0 = 191, Unknown1 = 1065353216, Unknown2 = 1071434956 });

            owner.EnqueueToVisible(new Server093A
            {
                UnitId = owner.Guid,
                unknownStructure0 = unknownStructure093A
            }, true);

            owner.MountId = Guid;

            // disable spawn animation
            SpawnAnimation = 2;
        }

        public void Disembark(Player owner)
        {
            owner.EnqueueToVisible(new ServerEntityCommand
            {
                Guid = owner.Guid,
                Time = 1,
                Unknown2 = true,
                Commands = new List<(EntityCommand, IEntityCommand)>
                {
                    (
                        EntityCommand.SetPlatform,
                        new SetPlatformCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetPosition,
                        new SetPositionCommand
                        {
                            Position = new Position(owner.Position), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetVelocityDefaults,
                        new SetVelocityDefaultsCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetMove,
                        new SetMoveCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetRotation,
                        new SetRotationCommand
                        {
                            Position = new Position(owner.Rotation), Unknown3 = false
                        }
                    ),
                    (
                        EntityCommand.SetStateDefault,
                        new SetStateDefaultCommand
                        {
                            Strafe = true
                        }
                    ),
                    (
                        EntityCommand.SetModeDefault,
                        new SetModeDefaultCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetState,
                        new SetStateCommand
                        {
                        }
                    ),
                    (
                        EntityCommand.SetVelocity,
                        new SetVelocityCommand
                        {
                            // FIXME: fill me
                        }
                    )
                }
            }, true);

            owner.EnqueueToVisible(new ServerDisembark
            {
                MountId = Guid,
                OwnerId = owner.Guid
            }, true);

            owner.Session.EnqueueMessageEncrypted(new Server0639
            {
            });

            // FIXME: this should probably be triggered by flair spell effects wearing off of the mount
            // NOTE: reverse order is important for hover boards!
            foreach (var itemVisual in itemVisuals.Reverse())
            {
                itemVisuals.Remove(itemVisual.Key);
                // as done on retail, flair by flair...
                UpdateVisuals();
            }

            owner.Session.EnqueueMessageEncrypted(new ServerMovementControl
            {
                Ticket = 1,
                Immediate = true,
                UnitId = owner.Guid
            });

            owner.MountId = 0;
        }
    }
}