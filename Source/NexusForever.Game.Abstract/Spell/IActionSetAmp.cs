using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IActionSetAmp : IDatabaseCharacter, IDatabaseState
    {
        EldanAugmentationEntry Entry { get; set; }
    }
}