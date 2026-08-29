namespace NexusForever.Game.Static.Spell
{
    public enum ShortcutType
    {
        None          = 0,
        BagItem       = 1, // DDBagItem
        Macro         = 2, // DDMacro
        GameCommand   = 3, // DDGameCommand
        SpellbookItem = 4, // DDSpellbookItem
        // Client has code paths for other values, but it seems these were collapsed into GameCommands and those paths are never called
    }
}
