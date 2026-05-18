using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPetEntity : IWorldEntity
    {
        uint OwnerGuid { get; }
        Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; }

        void Initialise(IPlayer owner, uint creature);
    }
}
