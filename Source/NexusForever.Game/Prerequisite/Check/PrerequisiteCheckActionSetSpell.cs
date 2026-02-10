using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.ActionSetSpell)]
    public class PrerequisiteCheckActionSetSpell : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckActionSetSpell(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            IActionSet actionSet = player.SpellManager.GetActionSet(player.SpellManager.ActiveActionSet);
            if (actionSet == null)
                return false;

            IActionSetShortcut shortcut = actionSet.GetShortcut(ShortcutType.SpellbookItem, objectId);
            bool hasShortcut = shortcut != null;

            return MatchBoolean(hasShortcut, comparison, PrerequisiteType.ActionSetSpell);
        }
    }
}
