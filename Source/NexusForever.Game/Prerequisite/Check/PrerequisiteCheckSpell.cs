using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.UnderSpell)]
    public class PrerequisiteCheckSpell : BasePrerequisiteHandler, IPrerequisiteCheck
    {
        #region Dependency Injection

        public PrerequisiteCheckSpell(
            ILogger<BasePrerequisiteHandler> log)
            : base(log)
        {
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (value == 0 && objectId == 0)
                return false;

            bool hasSpell = player.HasSpell(s => s.Spell4Id == value, out ISpell spell);
            return MatchBoolean(hasSpell, comparison, PrerequisiteType.UnderSpell);
        }
    }
}
