using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Static.Spell
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class CastResultVitalAttribute : Attribute
    {
        public Vital Vital { get; }

        public CastResultVitalAttribute(Vital vital)
        {
            Vital = vital;
        }
    }
}
