using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    public interface IDatabaseEntity
    {
        void Initialise(EntityModel model);
    }
}
