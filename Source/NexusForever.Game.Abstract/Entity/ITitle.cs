using NexusForever.Database.Character;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITitle : IDatabaseCharacter, IUpdate
    {
        ulong CharacterId { get; }
        CharacterTitleEntry Entry { get; }
        bool Revoked { get; set; }
        double? TimeRemaining { get; set; }
    }
}