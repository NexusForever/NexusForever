using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Compare the character's total spendable AMP points against the row's value.
    /// </summary>
    /// <remarks>
    /// The client's handler reads the same slot the AMP configuration validator spends against -
    /// the total, not what is left - as a 16 bit signed word and hands it to the unsigned
    /// comparator. The total is read from the active specialisation's budget here, at full
    /// <c>uint</c> width: the total can never reach 32767, so the client's narrower read can never
    /// disagree.
    ///
    /// This is the twin of <see cref="PrerequisiteCheckBonusPower"/>, but tests the whole pool -
    /// flat base, item bonus and Primal Matrix together - rather than the item bonus alone. No
    /// shipped prerequisite uses this type, so nothing reaches the check today; it is kept because
    /// it mirrors the client's own handler.
    /// </remarks>
    [PrerequisiteCheck(PrerequisiteType.Power)]
    public class PrerequisiteCheckPower : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckPower> log;

        public PrerequisiteCheckPower(
            ILogger<PrerequisiteCheckPower> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            IActionSet actionSet = player.SpellManager.GetActionSet(player.SpellManager.ActiveActionSet);

            uint state = actionSet.AmpPointLimit;

            switch (comparison)
            {
                case PrerequisiteComparison.Equal:
                    return state == value;
                case PrerequisiteComparison.NotEqual:
                    return state != value;
                case PrerequisiteComparison.GreaterThan:
                    return state > value;
                case PrerequisiteComparison.GreaterThanOrEqual:
                    return state >= value;
                case PrerequisiteComparison.LessThan:
                    return state < value;
                case PrerequisiteComparison.LessThanOrEqual:
                    return state <= value;
                default:
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.Power}!");
                    return false;
            }
        }
    }
}
