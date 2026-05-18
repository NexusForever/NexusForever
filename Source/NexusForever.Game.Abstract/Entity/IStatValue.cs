using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IStatValue
    {
        Stat Stat { get; }
        StatType Type { get; }
        float Value { get; set; }
        uint Data { get; set; }

        void SaveCharacter(ulong characterId, CharacterContext context);
    }
}