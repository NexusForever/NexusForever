using System;

namespace NexusForever.WorldServer.Game.Entity.Network
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EntityCommandAttribute : Attribute
    {
        public EntityCommand Command { get; }

        public EntityCommandAttribute(EntityCommand command)
        {
            Command = command;
        }
    }
}
