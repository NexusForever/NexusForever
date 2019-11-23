using System;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Game.Spell
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
