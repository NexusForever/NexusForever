﻿namespace NexusForever.Game.Static.Entity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StatAttribute : Attribute
    {
        public StatType Type { get; }
        public bool SendUpdate { get; }

        public StatAttribute(StatType type, bool sendUpdate = true)
        {
            Type       = type;
            SendUpdate = sendUpdate;
        }
    }
}
