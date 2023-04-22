using NexusForever.Database.Character;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPetFlair : IDatabaseCharacter
    {
        PetFlairEntry Entry { get; }
        ulong Owner { get; }
    }
}