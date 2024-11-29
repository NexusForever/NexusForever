namespace NexusForever.Game.Static.Prerequisite
{
    // TODO: name these from PrerequisiteType.tbl error messages
    public enum PrerequisiteType
    {
        None            = 0,
        Level           = 1,
        Race            = 2,
        Class           = 3,
        Faction         = 4,
        Reputation      = 5,
        Quest           = 6,
        Achievement     = 7,
        ItemProficiency = 8,
        Gender          = 10,
        Prerequisite    = 11,
        Path            = 52,
        Vital           = 73,
        SpellObj        = 129,
		/// <summary>
        /// Checks for an ObjectId, which is a hashed petflair id.
        /// </summary>
        HoverboardFlair = 190,
        /// <summary>
        /// Used for Mount checks
        /// </summary>
        Unknown194      = 194,
        /// <summary>
        /// Used for Mount checks
        /// </summary>
        Unknown195      = 195,
        SpellBaseId     = 214,
        Plane           = 232,
        BaseFaction     = 250,
        PurchasedTitle  = 288
    }
}
