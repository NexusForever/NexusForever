using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.SpellBaseId)]
    public class PrerequisiteCheckSpellBaseId : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckSpellBaseId> log;

        public PrerequisiteCheckSpellBaseId(
            ILogger<PrerequisiteCheckSpellBaseId> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch (comparison)
            {
                case PrerequisiteComparison.NotEqual:
                    return player.SpellManager.GetSpell(objectId) == null;
                case PrerequisiteComparison.Equal:
                    return player.SpellManager.GetSpell(objectId) != null;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.SpellBaseId}!");
                    return false;
            }
        }
    }
}
