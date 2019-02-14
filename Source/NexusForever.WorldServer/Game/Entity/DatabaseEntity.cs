using System;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseEntity : Attribute
    {
        public EntityType EntityType { get; }

        public DatabaseEntity(EntityType entityType)
        {
            EntityType = entityType;
        }
    }
}
