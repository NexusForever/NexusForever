using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.ActionSetSpell)]
    public class PrerequisiteCheckActionSetSpell : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckActionSetSpell> log;

        public PrerequisiteCheckActionSetSpell(
            ILogger<PrerequisiteCheckActionSetSpell> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IUnitEntity subject, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (subject is not IPlayer player)
                return false;

            IActionSet actionSet = player.SpellManager.GetActionSet(player.SpellManager.ActiveActionSet);
            if (actionSet == null)
                return false;

            IActionSetShortcut shortcut = actionSet.GetShortcut(ShortcutType.SpellbookItem, objectId);

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return shortcut != null;
                case PrerequisiteComparison.NotEqual:
                    return shortcut == null;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.ActionSetSpell}!");
                    return false;
            }
        }
    }
}
