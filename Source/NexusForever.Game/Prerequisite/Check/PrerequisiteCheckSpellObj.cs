using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.SpellObj)]
    internal class PrerequisiteCheckSpellObj : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckSpellObj> log;

        public PrerequisiteCheckSpellObj(
            ILogger<PrerequisiteCheckSpellObj> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (subject is not IPlayer player)
                return false;

            // TODO: Confirm how the objectId is calculated. It seems like this check always checks for a Spell that is determined by an objectId.

            // Error message is "Spell requirement not met"

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(value) != null;
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(value) == null;
                default:
                    log.LogWarning($"Unhandled {comparison} for {PrerequisiteType.SpellObj}!");
                    return false;
            }
        }
    }
}
