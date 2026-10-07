using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Shared;
using System.Numerics;

namespace NexusForever.Game.Prerequisite.Check
{
    // TODO: Consider adding a PrereuqisiteComparison collection to all handlers detailing for each handler which comparisons are valid.
    // Currently if for a handler only EQ and NEQ are valid but GT is used, it will still be handled even if it is not valid for that handler.
    public class BasePrerequisiteHandler : IBasePrerequisiteHandler
    {
        #region dependency injection

        public readonly ILogger<BasePrerequisiteHandler> log;

        public BasePrerequisiteHandler(ILogger<BasePrerequisiteHandler> log)
        {
            this.log = log;
        }

        #endregion


        public bool MatchEnum<T>(T entityValue, T value, PrerequisiteComparison comparison, PrerequisiteType type)
            where T : Enum
        {
            return MatchCompareable(entityValue.As<T, uint>(), value.As<T, uint>(), comparison, type);
        }

        public bool MatchCompareable<T>(T entityValue, T value, PrerequisiteComparison comparison, PrerequisiteType type)
            where T : IComparisonOperators<T, T, bool>
        {
            return comparison switch
            {
                PrerequisiteComparison.Equal => entityValue == value,
                PrerequisiteComparison.NotEqual => entityValue != value,
                PrerequisiteComparison.GreaterThanOrEqual => entityValue >= value,
                PrerequisiteComparison.GreaterThan => entityValue > value,
                PrerequisiteComparison.LessThanOrEqual => entityValue <= value,
                PrerequisiteComparison.LessThan => entityValue < value,
                _ => throw new InvalidOperationException($"Unhandled PrerequisiteComparison {comparison} for {type}!"),
            };
        }

        public bool MatchBoolean(bool entityValue, PrerequisiteComparison comparison, PrerequisiteType type)
        {
            return comparison switch
            {
                PrerequisiteComparison.Equal => entityValue,
                PrerequisiteComparison.NotEqual => !entityValue,
                _ => throw new InvalidOperationException($"Unhandled PrerequisiteComparison {comparison} for {type}! Only Equal and NotEqual are valid for boolean comparisons."),
            };
        }
    }
}