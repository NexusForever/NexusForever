namespace NexusForever.Game.Static.Quest
{
    [Flags]
    public enum QuestStateFlags
    {
        None               = 0x00,
        Tracked            = 0x02,
        Objective0Complete = 0x04,
        Objective1Complete = 0x08,
        Objective2Complete = 0x10,
        Objective3Complete = 0x20,
        Objective4Complete = 0x40,
        Objective5Complete = 0x80,
    }
}
