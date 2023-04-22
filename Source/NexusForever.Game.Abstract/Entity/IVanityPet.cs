using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IVanityPet : IWorldEntity
    {
        uint OwnerGuid { get; }
        Creature2Entry Creature { get; }
        Creature2DisplayGroupEntryEntry Creature2DisplayGroup { get; }
    }
}