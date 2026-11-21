using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Creature;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model.PlayerPath;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Entity
{
    public class PetEntity : WorldEntity, IPetEntity
    {
        private const float FollowDistance = 3f;
        private const float FollowMinRecalculateDistance = 5f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override EntityType Type => EntityType.Pet;

        public uint OwnerGuid { get; private set; }

        private readonly UpdateTimer followTimer = new(1d);

        #region Dependency Injection

        public PetEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory)
            : base(movementManager, entitySummonFactory)
        {
        }

        #endregion

        public void Initialise(IPlayer owner, ICreatureInfo creatureInfo)
        {
            OwnerGuid = owner.Guid;
            Initialise(creatureInfo);

            SetBaseProperty(Property.BaseHealth, 800.0f);

            SetStat(Static.Entity.Stat.Health, 800u);
            SetStat(Static.Entity.Stat.Level, 3u);
            SetStat(Static.Entity.Stat.Sheathed, 0u);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PetEntityModel
            {
                CreatureId  = CreatureId,
                OwnerId     = OwnerGuid,
                Name        = ""
            };
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            IPlayer owner = GetVisible<IPlayer>(OwnerGuid);
            if (owner == null)
            {
                // this shouldn't happen, log it anyway
                log.Error($"VanityPet {Guid} has lost it's owner {OwnerGuid}!");
                RemoveFromMap();
                return;
            }

            owner.VanityPetGuid = Guid;

            owner.EnqueueToVisible(new ServerPathScientistUnitScanParameters
            {
                UnitId = Guid,
                ScanRewardFlags  = 0,
                IsScannable  = true
            }, true);
        }

        public override void OnEnqueueRemoveFromMap()
        {
            followTimer.Reset(false);
            OwnerGuid = 0u;
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            Follow(lastTick);
        }

        private void Follow(double lastTick)
        {
            followTimer.Update(lastTick);
            if (!followTimer.HasElapsed)
                return;

            IPlayer owner = GetVisible<IPlayer>(OwnerGuid);
            if (owner == null)
            {
                // this shouldn't happen, log it anyway
                log.Error($"VanityPet {Guid} has lost it's owner {OwnerGuid}!");
                RemoveFromMap();
                return;
            }

            // only recalculate the path to owner if distance is significant
            float distance = owner.Position.GetDistance(Position);
            if (distance < FollowMinRecalculateDistance)
                return;

            MovementManager.Follow(owner, FollowDistance);

            followTimer.Reset();
        }
    }
}
