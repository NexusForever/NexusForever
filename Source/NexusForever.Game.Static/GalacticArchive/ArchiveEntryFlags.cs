namespace NexusForever.Game.Static.GalacticArchive
{
    [Flags]
    public enum ArchiveEntryFlags
    {
        None              = 0x00,
        /// <summary>
        /// Scientist text replaces the entry text for a scientist.
        /// </summary>
        ScientistOnly     = 0x01,
        /// <summary>
        /// Entry isn't shown in the article while it is locked.
        /// </summary>
        HiddenWhileLocked = 0x02,
        /// <summary>
        /// Entry is unlocked together with the article.
        /// </summary>
        UnlockWithArticle = 0x04
    }
}
