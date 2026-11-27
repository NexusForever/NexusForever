using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Entity
{
    internal sealed class TutorialConstructEntity(IMovementManager movementManager) : NonPlayerEntity(movementManager)
    {
        private const uint Creature = 73499u;
        private const uint ConstructZone = 5968u;
        private const uint Charge = 85535u;
        private const float ChargeRange = 7f;
        private const double ChargeCooldown = 12d;
        private const double RespawnDelay = 30d;

        private double nextCharge;
        private double respawn;

        internal static bool Matches(EntityModel model) =>
            model.World == 3460u && model.Creature == Creature && model.Type == EntityType.NonPlayer;

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            if (!InWorld)
                return;

            if (!IsAlive)
            {
                if (respawn > 0d && (respawn -= lastTick) <= 0d)
                {
                    Health = MaxHealth;
                    Shield = MaxShieldCapacity;
                    DeathState = null;
                }
                return;
            }

            nextCharge -= lastTick;
            if (nextCharge > 0d || GetActiveSpell(spell => spell.IsCasting) != null)
                return;

            IPlayer target = visibleEntities.Values.OfType<IPlayer>()
                .Where(player => Vector3.DistanceSquared(player.Position, Position) <= ChargeRange * ChargeRange && CanAttack(player))
                .MinBy(player => Vector3.DistanceSquared(player.Position, Position));
            if (target == null)
                return;

            if (TargetGuid != target.Guid)
                SetTarget(target);

            Vector3 offset = target.Position - Position;
            Rotation = new Vector3(MathF.Atan2(-offset.X, -offset.Z), 0f, 0f);
            CastSpell(Charge);
            nextCharge = ChargeCooldown;
        }

        public override bool CanAttack(IUnitEntity target)
        {
            if (target is not IPlayer { IsAlive: true, IsLoading: false, Zone.Id: ConstructZone } player ||
                !ReferenceEquals(player.Map, Map) || MathF.Abs(player.Position.Y - Position.Y) > 5f ||
                player.Faction1 != player.Faction2 || !base.CanAttack(target))
                return false;

            (ushort Quest, uint Goal) route = player.Faction1 switch
            {
                Faction.Exile => ((ushort)10520, 21296u),
                Faction.Dominion => ((ushort)10523, 21310u),
                _ => default
            };
            return route.Quest != 0u &&
                player.QuestManager.GetActiveQuests().Any(quest =>
                    quest.Id == route.Quest && quest.State == QuestState.Accepted && !quest.PendingDelete &&
                    quest.Count(goal => goal.Index < 3u && (goal.IsComplete() || goal.ObjectiveInfo.IsOptional())) == 3 &&
                    quest.Any(goal => goal.Index == 3u && !goal.IsComplete() && goal.ObjectiveInfo.Id == route.Goal &&
                        goal.ObjectiveInfo is { Type: QuestObjectiveType.KillCreature, Entry.Data: Creature }));
        }

        protected override void OnDeath()
        {
            if (DeathState != null)
                return;

            base.OnDeath();
            respawn = RespawnDelay;
            nextCharge = 0d;
        }
    }
}
