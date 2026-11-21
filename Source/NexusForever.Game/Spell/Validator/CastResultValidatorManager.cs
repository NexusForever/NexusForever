using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Validator;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell.Validator
{
    public class CastResultValidatorManager : ICastResultValidatorManager
    {
        #region Dependency Injection

        private readonly IEnumerable<ICastResultValidator> castResultValidators;

        public CastResultValidatorManager(
            IEnumerable<ICastResultValidator> castResultValidators)
        {
            this.castResultValidators = castResultValidators;
        }

        #endregion

        public CastResult GetCastResult(ISpell spell)
        {
            foreach (ICastResultValidator castResultValidator in castResultValidators)
            {
                CastResult result = castResultValidator.GetCastResult(spell);
                if (result != CastResult.Ok)
                    return result;
            }

            return CastResult.Ok;
        }
    }
}
