using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Quest;

namespace NexusForever.Game.Entity
{
    internal sealed class CryoHoloBot(IMovementManager movementManager, ILootDistributor lootDistributor)
        : NonPlayerEntity(movementManager, lootDistributor)
    {
        private double respawn;

        public override void Update(double lastTick)
        {
            base.Update(lastTick);
            if (!InWorld || IsAlive || respawn <= 0d || (respawn -= lastTick) > 0d)
                return;

            Health = MaxHealth;
            Shield = MaxShieldCapacity;
            DeathState = null;
        }

        public override bool CanAttack(IUnitEntity target) =>
            target is IPlayer player && CryoAwakening.CanFight(player, this) && base.CanAttack(target);

        protected override void RewardKiller(IPlayer player)
        {
            if (CryoAwakening.CanFight(player, this))
                base.RewardKiller(player);
        }

        protected override void OnDeath()
        {
            if (DeathState != null)
                return;

            base.OnDeath();
            respawn = 30d;
        }
    }
}
