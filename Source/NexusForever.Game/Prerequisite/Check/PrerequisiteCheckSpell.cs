using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.UnderSpell)]
    public class PrerequisiteCheckSpell : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckSpell> log;

        public PrerequisiteCheckSpell(
            ILogger<PrerequisiteCheckSpell> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (value == 0 && objectId == 0)
                return false;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return subject.HasSpell(s => s.Spell4Id == value, out ISpell equalSpell);
                case PrerequisiteComparison.NotEqual:
                    return !subject.HasSpell(s => s.Spell4Id == value, out ISpell notEqualSpell);
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.UnderSpell}!");
                    return false;
            }
        }
    }
}
