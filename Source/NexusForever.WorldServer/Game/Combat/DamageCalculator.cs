using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Combat
{
    public sealed class DamageCalculator : Singleton<DamageCalculator>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private float assaultRatingToPowerFormula = GameTableManager.Instance.GameFormula.GetEntry(1266).Datafloat0;
        private float supportRatingToPowerFormula = GameTableManager.Instance.GameFormula.GetEntry(1266).Datafloat01;

        public DamageCalculator()
        {
        }

        public void Initialise()
        {
            // Intentionally empty, but this could be a place to allow for setting of values from configuration, if allowing for damage/health modifications in config.
        }

        /// <summary>
        /// Returns the calculated damage and updates the referenced <see cref="SpellTargetInfo.SpellTargetEffectInfo"/> appropriately.
        /// </summary>
        /// <remarks>TODO: This should probably return an instance of a Class which describes all the damage done to both entities. Attackers can have reflected damage from this, etc.</remarks>
        public void CalculateDamage(
            UnitEntity attacker, 
            UnitEntity victim,
            Spell.Spell spell,
            ref SpellTargetInfo.SpellTargetEffectInfo info,
            DamageType damageType, 
            uint damage)
        {
            if (damage < 0)
                return;

            if (victim == null || !victim.IsAlive)
                return;

            SpellTargetInfo.SpellTargetEffectInfo.DamageDescription damageDescription = new SpellTargetInfo.SpellTargetEffectInfo.DamageDescription
            {
                DamageType   = damageType,
                CombatResult = CombatResult.Hit,
                RawDamage    = damage,
                RawScaledDamage = damage
            };
            CombatLogCastData castData = new CombatLogCastData
            {
                SpellId = spell.Spell4Id,
                CasterId = attacker.Guid,
                CombatResult = 0,
                TargetId = victim.Guid
            };

            if (CalculateDeflect(attacker, victim))
            {
                info.DropEffect = true;
                info.AddCombatLog(new CombatLogDeflect
                    {
                        BMultiHit = false,
                        CastData = castData
                    });
                return;
            }

            damage = GetBaseDamage(damage);
            damage = GetDamageAfterArmorMitigation(victim, damageType, damage);

            // TODO: Add in other attacking modifiers like Armor Pierce, Strikethrough, Multi-Hit, etc.

            if (CalculateCrit(ref damage, attacker, victim))
                damageDescription.CombatResult = CombatResult.Critical;

            uint preGlanceDamage = damage;
            if (CalculateGlance(ref damage, attacker, victim))
            {
                uint glanceDamage = preGlanceDamage - damage;
                // TODO: Add CombatLog
            }

            uint shieldedAmount = CalculateShieldAmount(damage, victim);
            damage -= shieldedAmount;
            damageDescription.ShieldAbsorbAmount = shieldedAmount;

            // TODO: Add in other defensive modifiers

            damageDescription.AdjustedDamage = damage;

            info.AddDamage(damageDescription);

            // TODO: Queue Proc Events
        }

        /// <summary>
        /// Get base damage value for the given <see cref="UnitEntity"/> with the provided parameter data from the <see cref="Spell4EffectsEntry"/>.
        /// </summary>
        public uint GetBaseDamageForSpell(UnitEntity attacker, float parameterType, float parameterValue)
        {
            switch (parameterType)
            {
                case 10:
                    return (uint)Math.Round(attacker.Level * parameterValue);
                case 12:
                    return (uint)Math.Round(GetAssaultPower(attacker) * parameterValue);
                case 13:
                    return (uint)Math.Round(GetSupportPower(attacker) * parameterValue);
            }

            return 0u;
        }

        private uint GetAssaultPower(UnitEntity attacker)
        {
            return (uint)Math.Round((float)attacker.GetPropertyValue(Property.AssaultRating) * assaultRatingToPowerFormula);
        }

        private uint GetSupportPower(UnitEntity attacker)
        {
            return (uint)Math.Round((float)attacker.GetPropertyValue(Property.SupportRating) * supportRatingToPowerFormula);
        }

        private uint GetBaseDamage(uint damage)
        {
            return (uint)(damage * (new Random().Next(95, 103) / 100f));
        }

        private uint GetDamageAfterArmorMitigation(UnitEntity victim, DamageType damageType, uint damage)
        {
            GameFormulaEntry armorFormulaEntry = GameTableManager.Instance.GameFormula.GetEntry(1234);
            float maximumArmorMitigation = (float)(armorFormulaEntry.Dataint01 * 0.01);
            float mitigationPct = (armorFormulaEntry.Datafloat0 / victim.Level * armorFormulaEntry.Datafloat01) * victim.GetPropertyValue(Property.Armor).Value / 100;

            if (damageType == DamageType.Physical)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetMagic).Value;
            else if (damageType == DamageType.Tech)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetTech).Value;
            else if (damageType == DamageType.Magic)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetMagic).Value;

            if (mitigationPct > 0f)
                damage = (uint)Math.Round(damage * (1f - Math.Clamp(mitigationPct, 0f, maximumArmorMitigation)));

            return damage;
        }

        private bool IsSuccessfulChance(float percentage)
        {
            return new Random().Next(1, 10000) <= percentage * 10000f;
        }

        /// <summary>
        /// Calculates and returns the shielded amount of damage.
        /// </summary>
        private uint CalculateShieldAmount(uint damage, UnitEntity victim)
        {
            uint maxShieldAmount = (uint)(damage * 0.625f); //GetPropertyValue(Property.ShieldMitigationMax).Value);
            uint shieldedAmount = maxShieldAmount >= victim.GetStatInteger(Stat.Shield).Value ? victim.GetStatInteger(Stat.Shield).Value : maxShieldAmount;

            return shieldedAmount;
        }

        /// <summary>
        /// Returns whether this attack was deflected.
        /// </summary>
        /// <remarks>Calculates chance to deflect an attack, avoiding all damage from that attack.</remarks>
        private bool CalculateDeflect(UnitEntity attacker, UnitEntity victim)
        {
            // TODO: Add in Strikethrough Calculations (and that increases Armor Pierce)

            float deflectChance = GetRatingPercentMod(Property.RatingAvoidIncrease, victim);
            return IsSuccessfulChance(deflectChance);
        }

        /// <summary>
        /// Returns whether this attack crit, and if so, modifies the referenced damage value appropriately.
        /// </summary>
        private bool CalculateCrit(ref uint damage, UnitEntity attacker, UnitEntity victim)
        {
            // TODO: Add in Crit Deflect and Critical Mitigation calculations

            float critRate = GetRatingPercentMod(Property.RatingCritChanceIncrease, attacker);
            if (critRate <= 0f)
                return false;

            bool crit = IsSuccessfulChance(critRate);
            if (crit)
                damage = (uint)Math.Round(damage * GetRatingPercentMod(Property.RatingCritSeverityIncrease, attacker));

            return crit;
        }

        /// <summary>
        /// Returns whether this attack was glanced, and if so, modifies the referenced damage value appropriately.
        /// </summary>
        private bool CalculateGlance(ref uint damage, UnitEntity attacker, UnitEntity victim)
        {
            float glanceChance = GetRatingPercentMod(Property.RatingGlanceChance, victim);
            if (glanceChance <= 0f)
                return false;

            bool glance = IsSuccessfulChance(glanceChance);
            if (glance)
                damage = (uint)Math.Round((float)(damage * (1 - GetRatingPercentMod(Property.RatingGlanceAmount, victim))));

            return glance;
        }

        private float GetRatingPercentMod(Property property, UnitEntity entity)
        {
            GameFormulaEntry gameFormula = null;

            switch (property)
            {
                case Property.Armor:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1234);
                    break;
                case Property.RatingArmorPierce:
                case Property.IgnoreArmorBase:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1269);
                    break;
                case Property.RatingAvoidReduce: // Strikethrough
                case Property.BaseAvoidReduceChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1230);
                    break;
                case Property.RatingAvoidIncrease: // Deflect
                case Property.BaseAvoidChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1235);
                    break;
                case Property.RatingCriticalMitigation:
                case Property.BaseCriticalMitigation:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1236);
                    break;
                case Property.RatingCritChanceDecrease: // Deflect Crit Chance
                case Property.BaseAvoidCritChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1236);
                    break;
                case Property.RatingCritChanceIncrease:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1231);
                    break;
                case Property.RatingCritSeverityIncrease:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1232);
                    break;
                case Property.CCDurationModifier:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1274);
                    break;
                case Property.RatingDamageReflectAmount:
                case Property.BaseDamageReflectAmount:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1272);
                    break;
                case Property.RatingDamageReflectChance:
                case Property.BaseDamageReflectChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1241);
                    break;
                case Property.RatingFocusRecovery:
                case Property.BaseFocusRecoveryInCombat:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1237);
                    break;
                case Property.RatingGlanceAmount:
                case Property.BaseGlanceAmount:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1271);
                    break;
                case Property.RatingGlanceChance:
                case Property.BaseGlanceChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1245);
                    break;
                case Property.RatingIntensity:
                case Property.BaseIntensity:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1243);
                    break;
                case Property.RatingLifesteal:
                case Property.BaseLifesteal:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1233);
                    break;
                case Property.RatingMultiHitAmount:
                case Property.BaseMultiHitAmount:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1270);
                    break;
                case Property.RatingMultiHitChance:
                case Property.BaseMultiHitChance:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1240);
                    break;
                case Property.RatingVigor:
                case Property.BaseVigor:
                    gameFormula = GameTableManager.Instance.GameFormula.GetEntry(1244);
                    break;
                default:
                    log.Warn($"Unhandled Property in calculating Percentage from Rating: {property}");
                    break;
            }

            // Return a decimal representing the % applied by this rating.
            float ratingMod = ((gameFormula.Datafloat0 / entity.Level) * gameFormula.Datafloat01) * entity.GetPropertyValue(property).Value;
            return Math.Min((GetBasePercentMod(property, entity) + ratingMod) * 100f, gameFormula.Dataint01) / 100f;
        }

        private float GetBasePercentMod(Property property, UnitEntity entity)
        {
            float baseValue = 0f;
            switch (property)
            {
                case Property.RatingArmorPierce:
                case Property.IgnoreArmorBase:
                    baseValue = entity.GetPropertyValue(Property.IgnoreArmorBase).Value;
                    break;
                case Property.RatingAvoidReduce: // Strikethrough
                case Property.BaseAvoidReduceChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidReduceChance).Value;
                    break;
                case Property.RatingAvoidIncrease: // Deflect
                case Property.BaseAvoidChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidChance).Value;
                    break;
                case Property.RatingCriticalMitigation:
                case Property.BaseCriticalMitigation:
                    baseValue = entity.GetPropertyValue(Property.BaseCriticalMitigation).Value;
                    break;
                case Property.RatingCritChanceDecrease: // Deflect Crit Chance
                case Property.BaseAvoidCritChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidCritChance).Value;
                    break;
                case Property.RatingCritChanceIncrease:
                case Property.BaseCritChance:
                    baseValue = entity.GetPropertyValue(Property.BaseCritChance).Value;
                    break;
                case Property.RatingCritSeverityIncrease:
                    // TODO: Confirm Property below
                    // baseValue = entity.GetPropertyValue(Property.CriticalHitSeverityMultiplier).Value;
                    break;
                case Property.RatingDamageReflectAmount:
                case Property.BaseDamageReflectAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseDamageReflectAmount).Value;
                    break;
                case Property.RatingDamageReflectChance:
                case Property.BaseDamageReflectChance:
                    baseValue = entity.GetPropertyValue(Property.BaseDamageReflectChance).Value;
                    break;
                case Property.RatingFocusRecovery:
                case Property.BaseFocusRecoveryInCombat:
                    baseValue = entity.GetPropertyValue(Property.BaseFocusRecoveryInCombat).Value;
                    break;
                case Property.RatingGlanceAmount:
                case Property.BaseGlanceAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseGlanceAmount).Value;
                    break;
                case Property.RatingGlanceChance:
                case Property.BaseGlanceChance:
                    baseValue = entity.GetPropertyValue(Property.BaseGlanceChance).Value;
                    break;
                case Property.RatingIntensity:
                case Property.BaseIntensity:
                    baseValue = entity.GetPropertyValue(Property.BaseIntensity).Value;
                    break;
                case Property.RatingLifesteal:
                case Property.BaseLifesteal:
                    baseValue = entity.GetPropertyValue(Property.BaseLifesteal).Value;
                    break;
                case Property.RatingMultiHitAmount:
                case Property.BaseMultiHitAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseMultiHitAmount).Value;
                    break;
                case Property.RatingMultiHitChance:
                case Property.BaseMultiHitChance:
                    baseValue = entity.GetPropertyValue(Property.BaseMultiHitChance).Value;
                    break;
                case Property.RatingVigor:
                case Property.BaseVigor:
                    baseValue = entity.GetPropertyValue(Property.BaseVigor).Value;
                    break;
                default:
                    log.Warn($"Unhandled Property in calculating Percentage from Base: {property}");
                    break;
            }

            return baseValue;
        }
    }
}
