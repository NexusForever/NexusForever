using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IEntityManager
    {
        void Initialise();

        StatAttribute GetStatAttribute(Stat stat);
    }
}