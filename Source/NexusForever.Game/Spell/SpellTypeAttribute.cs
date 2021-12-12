using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SpellTypeAttribute : Attribute
    {
        public CastMethod CastMethod { get; }

        public SpellTypeAttribute(CastMethod castMethod)
        {
            CastMethod = castMethod;
        }
    }
}
