using System;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VitalAttribute : Attribute
    {
        public Vital Vital { get; }

        public VitalAttribute(Vital vital)
        {
            Vital = vital;
        }
    }
}
