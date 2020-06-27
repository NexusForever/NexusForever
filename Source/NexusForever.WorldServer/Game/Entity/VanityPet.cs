using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VanityPet : WorldEntity
    {
        private const float FollowDistance = 3f;
        private const float FollowMinRecalculateDistance = 5f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public uint OwnerGuid { get; private set; }
        public Creature2Entry Creature { get; }
        public Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; }

        private readonly UpdateTimer followTimer = new UpdateTimer(1d);

        public VanityPet(Player owner, uint creature)
            : base(EntityType.Pet)
        {
            OwnerGuid               = owner.Guid;
            Creature                = GameTableManager.Instance.Creature2.GetEntry(creature);
            Creature2DisplayGroup   = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries.SingleOrDefault(x => x.Creature2DisplayGroupId == Creature.Creature2DisplayGroupId);
            DisplayInfo             = Creature2DisplayGroup?.Creature2DisplayInfoId ?? 0u;

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

            Player owner = GetVisible<Player>(OwnerGuid);
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
