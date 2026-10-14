namespace NexusForever.Game.Static.GalacticArchive
{
    [Flags]
    public enum ArchiveArticleFlags
    {
        None           = 0x00,
        /// <summary>
        /// Article can't be unlocked at the request of the client.
        /// </summary>
        NoClientUnlock = 0x01
    }
}
