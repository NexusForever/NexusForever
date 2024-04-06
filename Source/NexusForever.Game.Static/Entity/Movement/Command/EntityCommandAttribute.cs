namespace NexusForever.Game.Static.Entity.Movement.Command
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityCommandAttribute : Attribute
    {
        public EntityCommand Command { get; }

        public EntityCommandAttribute(EntityCommand command)
        {
            Command = command;
        }
    }
}
