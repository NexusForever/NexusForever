using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Combat;

namespace NexusForever.Game.Combat
{
    public sealed class DamageCalculator : IDamageCalculator
    {
        #region Dependency Injection

        private readonly ILogger<DamageCalculator> log;
        private readonly IGameTableManager gameTableManager;

        public DamageCalculator(
            ILogger<DamageCalculator> log,
            IGameTableManager gameTableManager)
        {
            this.log              = log;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        /// <summary>
        /// Returns the calculated damage and updates the referenced <see cref="SpellTargetInfo.SpellTargetEffectInfo"/> appropriately.
        /// </summary>
        /// <remarks>
        /// TODO: This should probably return an instance of a Class which describes all the damage done to both entities. Attackers can have reflected damage from this, etc.
        /// </remarks>
        public void CalculateDamage(IUnitEntity attacker, IUnitEntity victim, ISpell spell, ISpellTargetEffectInfo info)
        {
            IDamageDescription damageDescription = new SpellTargetInfo.SpellTargetEffectInfo.DamageDescription
            {
                DamageType   = info.Entry.DamageType,
                CombatResult = CombatResult.Hit
            };

            var castData = new CombatLogCastData
            {
                CasterId     = attacker.Guid,
                TargetId     = victim.Guid,
                SpellId      = spell.Parameters.SpellInfo.Entry.Id, // TODO: This was updated in order to use ISpell, check if correct
                CombatResult = CombatResult.Hit
            };

            if (CalculateDeflect(attacker, victim))
            {
                info.DropEffect = true;
                info.AddCombatLog(new CombatLogDeflect
                    {
                        BMultiHit = false,
                        CastData  = castData
                    });
                return;
            }

            uint damage = CalculateBaseDamage(attacker, victim, info.Entry);
            damageDescription.RawDamage       = damage;
            damageDescription.RawScaledDamage = damage;

            damage = CalculateBaseDamageVariance(damage);

            damage = GetDamageAfterArmorMitigation(victim, info.Entry.DamageType, damage);

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

            // TODO: Queue Proc Events*/
        }

        /// <summary>
        /// Get base damage value for the given <see cref="IUnitEntity"/> with the provided parameter data from the <see cref="Spell4EffectsEntry"/>.
        /// </summary>
        private uint CalculateBaseDamage(IUnitEntity caster, IUnitEntity target, Spell4EffectsEntry entry)
        {
            float basePropertyDamage = CalculateBasePropertyDamage(caster, entry);
            float baseEntityDamage   = CalculateBaseEntityDamage(caster, target, entry);

            float typeMultiplier = 1f;
            float typeBaseDamage = 0;
            switch (entry.EffectType)
            {
                case SpellEffectType.Transference:
                {
                    typeMultiplier = BitConverter.UInt32BitsToSingle(entry.DataBits02);
                    typeBaseDamage = entry.DataBits03;
                    break;
                }
                case SpellEffectType.Damage:
                case SpellEffectType.Heal:
                case SpellEffectType.DistanceDependentDamage:
                case SpellEffectType.DistributedDamage:
                case SpellEffectType.HealShields:
                case SpellEffectType.DamageShields:
                {
                    typeMultiplier = BitConverter.UInt32BitsToSingle(entry.DataBits00);
                    typeBaseDamage = entry.DataBits01;
                    break;
                }
            }

            if (caster.Type is not EntityType.Player and not EntityType.Ghost)
            {
                // TODO: some client code specific to non player entities
            }

            float baseDamage = basePropertyDamage + ((typeBaseDamage + baseEntityDamage) * typeMultiplier);

            float propertyMultiplier = caster.GetProperty((Property)(entry.DamageType + 140)).Value;

            return (uint)(propertyMultiplier * baseDamage);
        }

        private float CalculateBasePropertyDamage(IUnitEntity caster, Spell4EffectsEntry entry)
        {
            float GetProperty(Property property)
            {
                return caster.GetProperty(property)?.Value ?? 0f;
            }

            GameFormulaEntry forumulaEntry = gameTableManager.GameFormula.GetEntry(1266);

            float value = 0f;
            for (int i = 0; i < 4; i++)
            {
                switch (entry.ParameterType[i])
                {
                    case 1:
                        value += GetProperty(Property.Strength) * entry.ParameterValue[i];
                        break;
                    case 2:
                        value += GetProperty(Property.Dexterity) * entry.ParameterValue[i];
                        break;
                    case 3:
                        value += GetProperty(Property.Technology) * entry.ParameterValue[i];
                        break;
                    case 4:
                        value += GetProperty(Property.Magic) * entry.ParameterValue[i];
                        break;
                    case 5:
                        value += GetProperty(Property.Wisdom) * entry.ParameterValue[i];
                        break;
                    case 11:
                        value += GetProperty(Property.Stamina) * entry.ParameterValue[i];
                        break;
                    // client defaults to a value of 0.25f if the game table entry is missing
                    case 12:
                        value += GetProperty(Property.AssaultRating) * entry.ParameterValue[i] * forumulaEntry?.Datafloat0 ?? 0.25f;
                        break;
                    case 13:
                        value += GetProperty(Property.SupportRating) * entry.ParameterValue[i] * forumulaEntry?.Datafloat01 ?? 0.25f;
                        break;
                }
            }

            if (value >= 0f)
                return MathF.Ceiling(value);
            else
                return MathF.Floor(value);
        }

        private float CalculateBaseEntityDamage(IUnitEntity caster, IUnitEntity target, Spell4EffectsEntry entry)
        {
            float value = 0f;
            for (int i = 0; i < 4; i++)
            {
                switch (entry.ParameterType[i])
                {
                    // cases in the client not implemented
                    case 7:
                    case 8:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 21:
                    case 22:
                    case 23:
                    case 25:
                    case 26:
                        break;
                    case 10:
                    {
                        value += caster.Level * entry.ParameterValue[i];
                        break;
                    }
                }
            }

            return value;
        }

        private uint CalculateBaseDamageVariance(uint damage)
        {
            return (uint)(damage * (Random.Shared.Next(95, 103) / 100f));
        }

        private uint GetDamageAfterArmorMitigation(IUnitEntity victim, DamageType damageType, uint damage)
        {
            GameFormulaEntry armorFormulaEntry = gameTableManager.GameFormula.GetEntry(1234);
            float maximumArmorMitigation = (float)(armorFormulaEntry.Dataint01 * 0.01);
            float mitigationPct = (armorFormulaEntry.Datafloat0 / victim.Level * armorFormulaEntry.Datafloat01) * victim.GetPropertyValue(Property.Armor) / 100;

            if (damageType == DamageType.Physical)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetMagic);
            else if (damageType == DamageType.Tech)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetTech);
            else if (damageType == DamageType.Magic)
                mitigationPct += victim.GetPropertyValue(Property.DamageMitigationPctOffsetMagic);

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
        private uint CalculateShieldAmount(uint damage, IUnitEntity victim)
        {
            uint maxShieldAmount = (uint)(damage * victim.GetPropertyValue(Property.ShieldMitigationMax));
            uint shieldedAmount = Math.Min(victim.Shield, maxShieldAmount);

            return shieldedAmount;
        }

        /// <summary>
        /// Returns whether this attack was deflected.
        /// </summary>
        /// <remarks>Calculates chance to deflect an attack, avoiding all damage from that attack.</remarks>
        private bool CalculateDeflect(IUnitEntity attacker, IUnitEntity victim)
        {
            // TODO: Add in Strikethrough Calculations (and that increases Armor Pierce)

            float deflectChance = GetRatingPercentMod(Property.RatingAvoidIncrease, victim);
            return IsSuccessfulChance(deflectChance);
        }

        /// <summary>
        /// Returns whether this attack crit, and if so, modifies the referenced damage value appropriately.
        /// </summary>
        private bool CalculateCrit(ref uint damage, IUnitEntity attacker, IUnitEntity victim)
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
        private bool CalculateGlance(ref uint damage, IUnitEntity attacker, IUnitEntity victim)
        {
            float glanceChance = GetRatingPercentMod(Property.RatingGlanceChance, victim);
            if (glanceChance <= 0f)
                return false;

            bool glance = IsSuccessfulChance(glanceChance);
            if (glance)
                damage = (uint)Math.Round((float)(damage * (1 - GetRatingPercentMod(Property.RatingGlanceAmount, victim))));

            return glance;
        }

        private float GetRatingPercentMod(Property property, IUnitEntity entity)
        {
            GameFormulaEntry gameFormula = null;

            switch (property)
            {
                case Property.Armor:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1234);
                    break;
                case Property.RatingArmorPierce:
                case Property.IgnoreArmorBase:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1269);
                    break;
                case Property.RatingAvoidReduce: // Strikethrough
                case Property.BaseAvoidReduceChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1230);
                    break;
                case Property.RatingAvoidIncrease: // Deflect
                case Property.BaseAvoidChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1235);
                    break;
                case Property.RatingCriticalMitigation:
                case Property.BaseCriticalMitigation:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1236);
                    break;
                case Property.RatingCritChanceDecrease: // Deflect Crit Chance
                case Property.BaseAvoidCritChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1236);
                    break;
                case Property.RatingCritChanceIncrease:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1231);
                    break;
                case Property.RatingCritSeverityIncrease:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1232);
                    break;
                case Property.CCDurationModifier:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1274);
                    break;
                case Property.RatingDamageReflectAmount:
                case Property.BaseDamageReflectAmount:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1272);
                    break;
                case Property.RatingDamageReflectChance:
                case Property.BaseDamageReflectChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1241);
                    break;
                case Property.RatingFocusRecovery:
                case Property.BaseFocusRecoveryInCombat:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1237);
                    break;
                case Property.RatingGlanceAmount:
                case Property.BaseGlanceAmount:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1271);
                    break;
                case Property.RatingGlanceChance:
                case Property.BaseGlanceChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1245);
                    break;
                case Property.RatingIntensity:
                case Property.BaseIntensity:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1243);
                    break;
                case Property.RatingLifesteal:
                case Property.BaseLifesteal:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1233);
                    break;
                case Property.RatingMultiHitAmount:
                case Property.BaseMultiHitAmount:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1270);
                    break;
                case Property.RatingMultiHitChance:
                case Property.BaseMultiHitChance:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1240);
                    break;
                case Property.RatingVigor:
                case Property.BaseVigor:
                    gameFormula = gameTableManager.GameFormula.GetEntry(1244);
                    break;
                default:
                    log.LogWarning($"Unhandled Property in calculating Percentage from Rating: {property}");
                    break;
            }

            // Return a decimal representing the % applied by this rating.
            float ratingMod = ((gameFormula.Datafloat0 / entity.Level) * gameFormula.Datafloat01) * entity.GetPropertyValue(property);
            return Math.Min((GetBasePercentMod(property, entity) + ratingMod) * 100f, gameFormula.Dataint01) / 100f;
        }

        private float GetBasePercentMod(Property property, IUnitEntity entity)
        {
            float baseValue = 0f;
            switch (property)
            {
                case Property.RatingArmorPierce:
                case Property.IgnoreArmorBase:
                    baseValue = entity.GetPropertyValue(Property.IgnoreArmorBase);
                    break;
                case Property.RatingAvoidReduce: // Strikethrough
                case Property.BaseAvoidReduceChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidReduceChance);
                    break;
                case Property.RatingAvoidIncrease: // Deflect
                case Property.BaseAvoidChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidChance);
                    break;
                case Property.RatingCriticalMitigation:
                case Property.BaseCriticalMitigation:
                    baseValue = entity.GetPropertyValue(Property.BaseCriticalMitigation);
                    break;
                case Property.RatingCritChanceDecrease: // Deflect Crit Chance
                case Property.BaseAvoidCritChance:
                    baseValue = entity.GetPropertyValue(Property.BaseAvoidCritChance);
                    break;
                case Property.RatingCritChanceIncrease:
                case Property.BaseCritChance:
                    baseValue = entity.GetPropertyValue(Property.BaseCritChance);
                    break;
                case Property.RatingCritSeverityIncrease:
                    // TODO: Confirm Property below
                    // baseValue = entity.GetPropertyValue(Property.CriticalHitSeverityMultiplier);
                    break;
                case Property.RatingDamageReflectAmount:
                case Property.BaseDamageReflectAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseDamageReflectAmount);
                    break;
                case Property.RatingDamageReflectChance:
                case Property.BaseDamageReflectChance:
                    baseValue = entity.GetPropertyValue(Property.BaseDamageReflectChance);
                    break;
                case Property.RatingFocusRecovery:
                case Property.BaseFocusRecoveryInCombat:
                    baseValue = entity.GetPropertyValue(Property.BaseFocusRecoveryInCombat);
                    break;
                case Property.RatingGlanceAmount:
                case Property.BaseGlanceAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseGlanceAmount);
                    break;
                case Property.RatingGlanceChance:
                case Property.BaseGlanceChance:
                    baseValue = entity.GetPropertyValue(Property.BaseGlanceChance);
                    break;
                case Property.RatingIntensity:
                case Property.BaseIntensity:
                    baseValue = entity.GetPropertyValue(Property.BaseIntensity);
                    break;
                case Property.RatingLifesteal:
                case Property.BaseLifesteal:
                    baseValue = entity.GetPropertyValue(Property.BaseLifesteal);
                    break;
                case Property.RatingMultiHitAmount:
                case Property.BaseMultiHitAmount:
                    baseValue = entity.GetPropertyValue(Property.BaseMultiHitAmount);
                    break;
                case Property.RatingMultiHitChance:
                case Property.BaseMultiHitChance:
                    baseValue = entity.GetPropertyValue(Property.BaseMultiHitChance);
                    break;
                case Property.RatingVigor:
                case Property.BaseVigor:
                    baseValue = entity.GetPropertyValue(Property.BaseVigor);
                    break;
                default:
                    log.LogWarning($"Unhandled Property in calculating Percentage from Base: {property}");
                    break;
            }

            return baseValue;
        }
    }
}
