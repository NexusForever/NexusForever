using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite.Check
{
    /// <summary>
    /// Compare the character's item granted bonus AMP power against the row's value.
    /// </summary>
    /// <remarks>
    /// The client's handler reads its bonus power slot as a 16 bit signed word, sign extends it and
    /// hands the result to the unsigned comparator, so a negative bonus compares as
    /// <c>0xFFFFFFFF</c>, which is <b>not</b> <c>&lt; 10</c>: the gate fails and all further grants
    /// are locked out. The sign extension is mirrored exactly here; the server never stores a
    /// negative bonus, which is what makes the cap hold.
    ///
    /// Both shipped rows - 28557, the cast gate of <c>AMP Power Unlock</c>, and the apparently
    /// unused 43416 - are comparison id 5 against 10, i.e. a strict <c>bonusPower &lt; 10</c>, so
    /// the 11th grant is refused. objectId is not read by the client's handler.
    /// </remarks>
    [PrerequisiteCheck(PrerequisiteType.BonusPower)]
    public class PrerequisiteCheckBonusPower : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckBonusPower> log;

        public PrerequisiteCheckBonusPower(
            ILogger<PrerequisiteCheckBonusPower> log)
        {
            this.log = log;
        }

        #endregion

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            uint state = (uint)(int)player.SpellManager.BonusAmpPower;

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
                    log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.BonusPower}!");
                    return false;
            }
        }
    }
}
