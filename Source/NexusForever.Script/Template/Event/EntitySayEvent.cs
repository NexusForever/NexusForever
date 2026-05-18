using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Script.Template.Event
{
    public class EntitySayEvent : IEntitySayEvent
    {
        private IWorldEntity owner;
        private string message;

        public void Initialise(IWorldEntity owner, string message)
        {
            this.owner   = owner;
            this.message = message;
        }

        public void Invoke()
        {
            owner.NpcSay(message);
        }
    }
}
