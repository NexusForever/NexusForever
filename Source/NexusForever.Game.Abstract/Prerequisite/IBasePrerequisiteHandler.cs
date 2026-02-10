using NexusForever.Game.Static.Prerequisite;
using System.Numerics;

namespace NexusForever.Game.Abstract.Prerequisite
{
    public interface IBasePrerequisiteHandler
    {
        public bool MatchEnum<T>(T entityValue, T value, PrerequisiteComparison comparison, PrerequisiteType type)
            where T : Enum;
        public bool MatchCompareable<T>(T entityValue, T value, PrerequisiteComparison comparison, PrerequisiteType type)
            where T : IComparisonOperators<T, T, bool>;
        public bool MatchBoolean(bool entityValue, PrerequisiteComparison comparison, PrerequisiteType type);
    }
}
