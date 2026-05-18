using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IActionSetShortcut : IDatabaseCharacter, IDatabaseState
    {
        UILocation Location { get; }
        ShortcutType ShortcutType { get; set; }
        uint ObjectId { get; set; }
        byte Tier { get; set; }
    }
}