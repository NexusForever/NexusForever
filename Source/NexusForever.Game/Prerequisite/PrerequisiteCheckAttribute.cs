using NexusForever.Game.Static.Prerequisite;

namespace NexusForever.Game.Prerequisite
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PrerequisiteCheckAttribute : Attribute
    {
        public PrerequisiteType Type { get; }

        public PrerequisiteCheckAttribute(PrerequisiteType type)
        {
            Type = type;
        }
    }
}
