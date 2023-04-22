using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseEntityAttribute : Attribute
    {
        public EntityType EntityType { get; }

        public DatabaseEntityAttribute(EntityType entityType)
        {
            EntityType = entityType;
        }
    }
}
