using System;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CastMethodHandlerAttribute : Attribute
    {
        public CastMethod CastMethod { get; }

        public CastMethodHandlerAttribute(CastMethod castMethod)
        {
            CastMethod = castMethod;
        }
    }
}
