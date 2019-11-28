namespace NexusForever.WorldServer.Game.Prerequisite.Static
{
    // TODO: name these from PrerequisiteType.tbl error messages
    public enum PrerequisiteType
    {
        None        = 0,
        Level       = 1,
        Race        = 2,
        Class       = 3,
        Faction     = 4,
        Reputation  = 5,
        Quest       = 6,
        Achievement = 7,
        /// <summary>
        /// Checks for whether or not the Player has a Spell. Used in cases to check for if player has AMP.
        /// </summary>
        Spell       = 15,
        /// <summary>
        /// Appears to check whether the user is in combat. Error Msg: "You must be in combat"
        /// </summary>
        InCombat    = 28,
        /// <summary>
        /// Checks for whether or not the Player has an AMP.
        /// </summary>
        AMP         = 50,
        Path        = 52,
        /// <summary>
        /// Checks for an objectId. Used in the "RavelSignal" SpellEffectType at minimum. Error: World requirement not met
        /// </summary>
        WorldReq    = 109,
        Stealth     = 116,
        SpellObj    = 129,
        SpellBaseId = 214,
        Faction2    = 243,
        BaseFaction = 250
    }
}
