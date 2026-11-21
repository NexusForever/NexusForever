using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell.Validator
{
    public class CCCastResultValidator : ICastResultValidator
    {
        public CastResult GetCastResult(ISpell spell)
        {
            if (spell.Parameters.SpellInfo.CasterCCConditions != null)
            {
                CastResult? result = GetCasterCCResult(spell.Caster, spell.Parameters.SpellInfo.CasterCCConditions.CcStateMask);
                if (result != null)
                    return result.Value;

                // doesn't seem to be used anywhere in the client
                // Parameters.SpellInfo.CasterCCConditions.CcStateFlagsRequired
            }

            if (spell.Parameters.PrimaryTargetId != 0
                && spell.Parameters.SpellInfo.TargetCCConditions != null)
            {
                IUnitEntity target = spell.Caster.GetVisible<IUnitEntity>(spell.Parameters.PrimaryTargetId);
                if (target != null)
                {
                    CastResult? result = GetTargetCCResult(target, spell.Parameters.SpellInfo.TargetCCConditions.CcStateMask);
                    if (result != null)
                        return result.Value;

                    // doesn't seem to be used anywhere in the client
                    // Parameters.SpellInfo.TargetCCConditions.CcStateFlagsRequired
                }
            }

            return CastResult.Ok;
        }

        private CastResult? GetCasterCCResult(IUnitEntity target, uint mask)
        {
            foreach (CCState ccState in target.CrowdControlManager.GetCCStates())
            {
                var flag = 1 << (int)ccState;
                if ((mask & flag) != 0)
                {
                    return ccState switch
                    {
                        CCState.Stun                 => CastResult.CCStun,
                        CCState.Sleep                => CastResult.CCStun,
                        CCState.Root                 => CastResult.CCRoot,
                        CCState.Disarm               => CastResult.CCDisarm,
                        CCState.Silence              => CastResult.CCSilence,
                        CCState.Polymorph            => CastResult.CCPolymorph,
                        CCState.Fear                 => CastResult.CCFear,
                        CCState.Hold                 => CastResult.CCHold,
                        CCState.Knockdown            => CastResult.CCKnockdown,
                        CCState.Vulnerability        => CastResult.CCVulnerability,
                        CCState.VulnerabilityWithAct => CastResult.CCVulnerabilityWithAct,
                        CCState.Disorient            => CastResult.CCDisorient,
                        CCState.Disable              => CastResult.CCDisable,
                        CCState.Taunt                => CastResult.CCTaunt,
                        CCState.DeTaunt              => CastResult.CCDeTaunt,
                        CCState.Blind                => CastResult.CCBlind,
                        CCState.Knockback            => CastResult.CCKnockback,
                        CCState.Pushback             => CastResult.CCPushback,
                        CCState.Pull                 => CastResult.CCPull,
                        CCState.PositionSwitch       => CastResult.CCPositionSwitch,
                        CCState.Tether               => CastResult.CCTether,
                        CCState.Snare                => CastResult.CCSnare,
                        CCState.Interrupt            => CastResult.CCInterrupt,
                        CCState.Daze                 => CastResult.CCDaze,
                        CCState.Subdue               => CastResult.CCSubdue,
                        CCState.Grounded             => CastResult.CCGrounded,
                        CCState.DisableCinematic     => CastResult.CCDisableCinematic,
                        CCState.AbilityRestriction   => CastResult.CCAbilityRestriction,
                        _                            => CastResult.CCStun
                    };
                }
            }

            return null;
        }

        private CastResult? GetTargetCCResult(IUnitEntity target, uint mask)
        {
            foreach (CCState ccState in target.CrowdControlManager.GetCCStates())
            {
                var flag = 1 << (int)ccState;
                if ((mask & flag) != 0)
                {
                    return ccState switch
                    {
                        CCState.Stun                 => CastResult.TargetCannotBeStun,
                        CCState.Sleep                => CastResult.TargetCannotBeSleep,
                        CCState.Root                 => CastResult.TargetCannotBeRoot,
                        CCState.Disarm               => CastResult.TargetCannotBeDisarm,
                        CCState.Silence              => CastResult.TargetCannotBeSilence,
                        CCState.Polymorph            => CastResult.TargetCannotBePolymorph,
                        CCState.Fear                 => CastResult.TargetCannotBeFear,
                        CCState.Hold                 => CastResult.TargetCannotBeHold,
                        CCState.Knockdown            => CastResult.TargetCannotBeKnockdown,
                        CCState.Vulnerability        => CastResult.TargetCannotBeVulnerability,
                        CCState.VulnerabilityWithAct => CastResult.TargetCannotBeVulnerability,
                        CCState.Disorient            => CastResult.TargetCannotBeDisorient,
                        CCState.Disable              => CastResult.TargetCannotBeDisable,
                        CCState.Taunt                => CastResult.TargetCannotBeTaunt,
                        CCState.DeTaunt              => CastResult.TargetCannotBeDeTaunt,
                        CCState.Blind                => CastResult.TargetCannotBeBlind,
                        CCState.Knockback            => CastResult.TargetCannotBeKnockback,
                        CCState.Pushback             => CastResult.TargetCannotBePushback,
                        CCState.Pull                 => CastResult.TargetCannotBePull,
                        CCState.PositionSwitch       => CastResult.TargetCannotBePositionSwitch,
                        CCState.Tether               => CastResult.TargetCannotBeTether,
                        CCState.Snare                => CastResult.TargetCannotBeSnare,
                        CCState.Interrupt            => CastResult.TargetCannotBeInterrupt,
                        CCState.Daze                 => CastResult.TargetCannotBeDaze,
                        CCState.Subdue               => CastResult.TargetCannotBeSubdue,
                        CCState.Grounded             => CastResult.TargetCannotBeGrounded,
                        CCState.DisableCinematic     => CastResult.TargetCannotBeDisableCinematic,
                        CCState.AbilityRestriction   => CastResult.TargetCannotBeAbilityRestriction,
                        _                            => CastResult.TargetCannotBeStun
                    };
                }
            }

            return null;
        }
    }
}
