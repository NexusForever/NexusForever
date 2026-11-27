using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Game.Static.Entity.Movement.Spline;

namespace NexusForever.Game.Entity
{
    /// <summary>Grimfang Forest wildlife used by the Murkmire hunts.</summary>
    internal sealed class MurkmireWildlifeEntity(IMovementManager movement, ILootDistributor loot)
        : NonPlayerEntity(movement, loot)
    {
        private const uint AutoAttackSpell = 72135;
        private const float AggroRangeSquared = 8f * 8f;
        private const float LeashRangeSquared = 15f * 15f;
        private double nextAction;
        private Vector3? destination;

        internal static bool Matches(EntityModel model) => model is
        {
            Type: EntityType.NonPlayer,
            World: 51,
            Area: 2247 or 2248 or 2249 or 2250,
            Creature: 41480 or 41497 or 41525 or 41535 or 41545
        };

        protected override void OnDeath()
        {
            base.OnDeath();
            StopMoving(MovementManager.GetPosition());
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            if (!InWorld || !IsAlive)
                return;

            if ((nextAction -= lastTick) > 0d)
                return;
            nextAction = 0.5d;

            Vector3 current = MovementManager.GetPosition();
            IPlayer enemy = TargetGuid is { } guid ? GetVisible<IPlayer>(guid) : null;
            if (!CanPursue(enemy) || Vector3.DistanceSquared(current, LeashPosition) > LeashRangeSquared)
            {
                if (TargetGuid.HasValue)
                {
                    SetTarget((IWorldEntity)null);
                    ThreatManager.ClearThreatList();
                }
                if (Vector3.DistanceSquared(current, LeashPosition) > 2f * 2f)
                {
                    MoveTo(current, LeashPosition);
                    return;
                }
                enemy = GetVisible().OfType<IPlayer>().Where(player => CanPursue(player) &&
                    Vector3.DistanceSquared(current, player.Position) <= AggroRangeSquared)
                    .MinBy(player => Vector3.DistanceSquared(current, player.Position));
                if (enemy == null)
                    return;
                SetTarget(enemy);
            }

            Vector3 offset = enemy.Position - current;
            offset.Y = 0f;
            float distance = offset.Length();
            if (distance > 4f)
            {
                Vector3 step = current + offset / distance * MathF.Min(2f, distance - 3f);
                if (Map.GetTerrainHeight(step.X, step.Z) is { } ground && MathF.Abs(ground - current.Y) <= 1.5f)
                {
                    step.Y = ground;
                    MoveTo(current, step);
                }
                return;
            }

            if (GetActiveSpell(spell => spell.IsCasting) != null)
                return;

            StopMoving(current);
            MovementManager.SetRotationFaceUnit(enemy.Guid);
            CastSpell(AutoAttackSpell, new SpellParameters { PrimaryTargetId = enemy.Guid });
            nextAction = 3d;
        }

        private bool CanPursue(IPlayer player) =>
            player is { InWorld: true, IsAlive: true, IsLoading: false } &&
            ReferenceEquals(player.Map, Map) &&
            Vector3.DistanceSquared(player.Position, LeashPosition) <= LeashRangeSquared &&
            CanAttack(player);

        private void StopMoving(Vector3 position)
        {
            if (!destination.HasValue)
                return;

            MovementManager.SetPosition(position, false);
            MovementManager.SetMoveDefaults(false);
            MovementManager.SetStateDefault();
            destination = null;
        }

        private void MoveTo(Vector3 current, Vector3 target)
        {
            if (destination is { } previous && Vector3.DistanceSquared(previous, target) < 1f)
                return;
            MovementManager.SetMode(ModeType.Walk);
            MovementManager.LaunchSpline([current, target], SplineType.Linear, SplineMode.OneShot, 3f);
            destination = target;
        }
    }
}
