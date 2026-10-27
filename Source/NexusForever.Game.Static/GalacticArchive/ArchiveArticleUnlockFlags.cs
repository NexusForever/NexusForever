namespace NexusForever.Game.Static.GalacticArchive
{
    [Flags]
    public enum ArchiveArticleUnlockFlags : uint
    {
        None     = 0x00000000,
        /// <summary>
        /// Mask of the entry bits, bit N unlocks the entry in slot N of the article.
        /// </summary>
        Entries  = 0x0000FFFF,
        Viewed   = 0x40000000,
        Unlocked = 0x80000000
    }
}
