using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
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
