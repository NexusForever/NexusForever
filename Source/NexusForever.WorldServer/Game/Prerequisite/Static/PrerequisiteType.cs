namespace NexusForever.WorldServer.Game.Prerequisite.Static
{
    // TODO: name these from PrerequisiteType.tbl error messages
    public enum PrerequisiteType
    {
        None         = 0,
        Level        = 1,
        Race         = 2,
        Class        = 3,
        Faction      = 4,
        Reputation   = 5,
        Quest        = 6,
        Achievement  = 7,
        Prerequisite = 11,
        Path         = 52,
        Vital        = 73,
        SpellObj     = 129,
        /// <summary>
        /// Used for Mount checks
        /// </summary>
        Unknown194   = 194,
        /// <summary>
        /// Used for Mount checks
        /// </summary>
        Unknown195   = 195,
        SpellBaseId  = 214,
        Plane        = 232,
        BaseFaction  = 250
    }
}
