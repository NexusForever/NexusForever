using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VanityPet : UnitEntity
    {
        public uint OwnerGuid { get; }
        public Creature2Entry Creature { get; }
        public Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; }

        public VanityPet(Player owner, uint creature)
            : base(EntityType.Pet)
        {
            OwnerGuid               = owner.Guid;
            Creature                = GameTableManager.Creature2.GetEntry(creature);
            Creature2DisplayGroup   = GameTableManager.Creature2DisplayGroupEntry.Entries.SingleOrDefault(x => x.Creature2DisplayGroupId == Creature.Creature2DisplayGroupId);
            DisplayInfo             = Creature2DisplayGroup.Creature2DisplayInfoId;

            SetProperty(Property.BaseHealth, 800.0f, 800.0f);

            SetStat(Stat.Health, 800u);
            SetStat(Stat.Level, 3u);
            SetStat(Stat.Sheathed, 0u);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PetEntityModel
            {
                CreatureId  = Creature.Id,
                OwnerId     = OwnerGuid,
                Name        = ""
            };
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);
            Player owner = GetVisible<Player>(OwnerGuid);
            owner.PetGuid = Guid;

            owner.EnqueueToVisible(new Server08B3
            {
                MountGuid = Guid,
                Unknown0  = 0,
                Unknown1  = true
            }, true);
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            var owner = Map.GetEntity<Player>(OwnerGuid);

            var entityCreate = base.BuildCreatePacket();
            entityCreate.Time = 904575;
            entityCreate.Faction1 = Faction1;
            entityCreate.Faction2 = Faction2;

            entityCreate.Commands = new Dictionary<EntityCommand, IEntityCommand>
            {
                {
                    EntityCommand.SetPlatform,
                    new SetPlatformCommand
                    {
                        //Platform = OwnerGuid
                    }
                },
                {
                    EntityCommand.SetPosition,
                    new SetPositionCommand
                    {
                        Position = new Position(owner.Position)
                    }
                },
                {
                    EntityCommand.SetVelocity,
                    new SetVelocityCommand
                    {
                    }
                },
                {
                    EntityCommand.SetMove,
                    new SetMoveCommand
                    {
                    }
                },
                {
                    EntityCommand.SetRotation,
                    new SetRotationCommand
                    {
                        Position = new Position(owner.Rotation)
                    }
                },
                {
                    EntityCommand.SetState,
                    new SetStateCommand
                    {
                        State = 257
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
    }
}
