using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell.Type
{
    /// <summary>
    /// A cast that trades something the caster owns for what the spell grants, such as an unlock item.
    /// </summary>
    /// <remarks>
    /// The client has no code that distinguishes this cast method from <see cref="CastMethod.Normal"/>:
    /// there is no cast bar, no channel, no threshold and no client-side confirmation step, and every
    /// spell that uses it has no cast time, no cooldown and no cost. The timeline is therefore
    /// <see cref="SpellNormal"/>'s; what this cast method adds is server-side, in a later commit.
    /// </remarks>
    public class SpellTransactional : SpellNormal
    {
        public override CastMethod CastMethod => CastMethod.Transactional;

        #region Dependency Injection

        private readonly ILogger<SpellTransactional> log;

        public SpellTransactional(
            ILogger<SpellTransactional> log,
            ISpellTargetInfoCollection spellTargetInfoCollection,
            IGlobalSpellManager globalSpellManager,
            ICastResultValidatorManager castResultValidatorManager,
            IDisableManager disableManager)
            : base(log, spellTargetInfoCollection, globalSpellManager, castResultValidatorManager, disableManager)
        {
            this.log = log;
        }

        #endregion
    }
}
