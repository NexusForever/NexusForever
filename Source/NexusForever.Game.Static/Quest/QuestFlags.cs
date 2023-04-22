namespace NexusForever.Game.Static.Quest
{
    [Flags]
    public enum QuestFlags
    {
        None         = 0x0000,
        AutoComplete = 0x0001,
        Optional     = 0x0008,
    }
}
