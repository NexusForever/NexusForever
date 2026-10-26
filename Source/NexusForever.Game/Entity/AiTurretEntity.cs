using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class AiTurretEntity : CreatureEntity, IAiTurretEntity
    {
        public override EntityType Type => EntityType.AiTurret;

        private static readonly HashSet<uint> NpeSimTurretCreatureIds = [73494u, 74862u];
        private const uint MomentOfOpportunitySpellId = 85472u;
        private const uint ChargingDominionSpellId    = 85469u;
        private const uint ChargingExileSpellId       = 86978u;

        private DateTime vulnerableUntil = DateTime.MinValue;

        #region Dependency Injection

        public AiTurretEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new AiTurretEntityModel
            {
                CreatureId        = CreatureId,
                QuestChecklistIdx = 0
            };
        }

        /// <summary>
        /// NPE sim-turrets are immune until their Charging cast is interrupted (any hit during the cast).
        /// </summary>
        public override void TakeDamage(IUnitEntity attacker, IDamageDescription damageDescription)
        {
            if (!NpeSimTurretCreatureIds.Contains(CreatureId))
            {
                base.TakeDamage(attacker, damageDescription);
                return;
            }

            // Soft interrupt: any hit while the turret is casting (Charging) opens the kill window.
            // Falls back to any active cast in case spell id tables differ by faction/build.
            ISpell charging = GetActiveSpell(s => s.IsCasting && IsChargingSpell(s.Parameters.SpellInfo.Entry.Id))
                ?? GetActiveSpell(s => s.IsCasting);
            if (charging != null)
            {
                CancelSpellCast(charging.CastingId);
                CastSpell(MomentOfOpportunitySpellId, new SpellParameters
                {
                    UserInitiatedSpellCast = false,
                    PrimaryTargetId        = Guid
                });
                vulnerableUntil = DateTime.UtcNow.AddSeconds(15);
                base.TakeDamage(attacker, damageDescription);
                return;
            }

            if (DateTime.UtcNow < vulnerableUntil)
            {
                base.TakeDamage(attacker, damageDescription);
                return;
            }

            // Immune: generate threat so the turret stays in combat and will re-charge.
            ThreatManager.UpdateThreat(attacker, (int)Math.Max(1u, damageDescription.RawDamage));
        }

        private static bool IsChargingSpell(uint spell4Id)
        {
            return spell4Id is ChargingDominionSpellId or ChargingExileSpellId;
        }
    }
}
