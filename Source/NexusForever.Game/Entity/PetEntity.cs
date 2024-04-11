using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;
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
        public Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; private set; }

        private readonly UpdateTimer followTimer = new(1d);

        #region Dependency Injection

        public PetEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        public void Initialise(IPlayer owner, uint creature)
        {
            OwnerGuid = owner.Guid;
            Initialise(creature);

            Creature2DisplayGroup = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries.SingleOrDefault(x => x.Creature2DisplayGroupId == CreatureEntry.Creature2DisplayGroupId);
            SetVisualInfo(Creature2DisplayGroup?.Creature2DisplayInfoId ?? 0u, 0);

            SetBaseProperty(Property.BaseHealth, 800.0f);

            SetStat(Stat.Health, 800u);
            SetStat(Stat.Level, 3u);
            SetStat(Stat.Sheathed, 0u);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new PetEntityModel
            {
                CreatureId  = CreatureEntry.Id,
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

            owner.EnqueueToVisible(new Server08B3
            {
                MountGuid = Guid,
                Unknown0  = 0,
                Unknown1  = true
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
