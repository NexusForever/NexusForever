using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Does the character's level grant AMP points at all?
    /// </summary>
    /// <remarks>
    /// The client's handler reads neither the row's objectId nor its value: it tests the character's
    /// level against the AMP point level floor of the same <c>GameFormula</c> row the point budget
    /// uses, and honours comparison ids 1 and 2 only, as a membership test. Both shipped rows -
    /// 23592 and 38331 - are comparison id 1, each paired in slot 1 with an explicit level check
    /// (<c>&gt;= 11</c> and <c>== 10</c>).
    ///
    /// Both rows are referenced only by <c>LevelUpUnlock</c> (36 -> 38331, exactly level 10,
    /// "+7 AMP Power"; 191 -> 23592, level 11 and above, "+1 AMP Power"); no spell or item table
    /// uses either, so nothing in NexusForever evaluates this check until a level-up-unlock
    /// feature exists.
    ///
    /// The client's item, item2 and reputation paths pass 0 as the level to this check; we read
    /// the player's level directly. No shipped row can reach those paths.
    ///
    /// Note that the client's own test is a <b>signed</b> <c>&gt;=</c> while the point budget uses
    /// an unsigned <c>&lt;</c> against the same floor; the two agree for every level a character can
    /// hold, so the difference only matters for a level the server could never set.
    /// </remarks>
    [PrerequisiteCheck(PrerequisiteType.LevelGrantAmpPoints)]
    public class PrerequisiteCheckLevelGrantAmpPoints : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckLevelGrantAmpPoints> log;

        public PrerequisiteCheckLevelGrantAmpPoints(
            ILogger<PrerequisiteCheckLevelGrantAmpPoints> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            bool granted = (int)player.Level >= (int)ActionSet.AmpPointLevelFloor;

            // by raw comparison id, not by the enum names: this check is a membership test, so 1
            // means "grants points" and 2 means "does not" - neither is the equality the names say.
            switch ((uint)comparison)
            {
                case 1u:
                    return granted;
                case 2u:
                    return !granted;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.LevelGrantAmpPoints}!");
                    return false;
            }
        }
    }
}
