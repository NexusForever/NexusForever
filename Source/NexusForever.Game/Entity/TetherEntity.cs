using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Stat;
using NexusForever.Game.Abstract.Loot.Distribute;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model.Combat.CrowdControl;
using NexusForever.Shared;

namespace NexusForever.Game.Entity
{
    public class TetherEntity : NonPlayerEntity, ITetherEntity
    {
        private enum Spell
        {
            TetherBeam = 47447
        }

        #region Dependency Injection

        private readonly IFactory<ISpellParameters> spellParameterFactory;

        public TetherEntity(IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory,
            IStatUpdateManager<IUnitEntity> statUpdateManager,
            ISpellFactory spellFactory,
            ILootDistributor lootDistributor,
            IFactory<ISpellParameters> spellParameterFactory)
            : base(movementManager, entitySummonFactory, statUpdateManager, spellFactory, lootDistributor)
        {
            this.spellParameterFactory = spellParameterFactory;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="ITetherEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            if (SummonerGuid == null)
                return;

            IPlayer summoner = map.GetEntity<IPlayer>(SummonerGuid.Value);
            if (summoner == null)
                return;

            ISpellParameters parameters = spellParameterFactory.Resolve();
            parameters.PrimaryTargetId = SummonerGuid.Value;
            CastSpell(Spell.TetherBeam, parameters);

            summoner.Session.EnqueueMessageEncrypted(new ServerEntityCCTetherUnit
            {
                UnitId = guid
            });
        }

        /// <summary>
        /// Returns whether or not this <see cref="IUnitEntity"/> is an attackable target for supplied <see cref="IUnitEntity"/>.
        /// </summary>
        public override bool IsValidAttackTarget(IUnitEntity attacker)
        {
            if (SummonerGuid == null)
                return false;

            IPlayer summoner = GetVisible<IPlayer>(SummonerGuid.Value);
            if (summoner == null)
                return false;

            // can only attack a tether if you are friendly to the summoner
            return attacker.GetDispositionTo(summoner.Faction1) == Disposition.Friendly;
        }

        protected override void OnDeath(IUnitEntity killer)
        {
            base.OnDeath(killer);

            if (SummonerGuid == null)
                return;

            IPlayer summoner = GetVisible<IPlayer>(SummonerGuid.Value);
            if (summoner == null)
                return;

            ISpellTargetEffectInfo effectInfo = summoner.CrowdControlManager.GetCCEffect(CCState.Tether);
            if (effectInfo == null)
                return;

            effectInfo.Finish();
        }
    }
}
