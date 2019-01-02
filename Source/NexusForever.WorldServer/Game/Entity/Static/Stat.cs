namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum Stat
    {
        Health              = 0,
        Focus               = 1,    // Must be float. How do we enforce?
        /// <summary>
        /// Endurance
        /// </summary>
        Resource0           = 2,    // Must be float. How do we enforce?
        /// <summary>
        /// Kinetic Energy (Warrior), Psi Points (Esper), Volatile Energy (Engineer), Medic Cores (Medic)
        /// </summary>
        Resource1           = 3,    // Must be float. How do we enforce?
        Resource2           = 4,    // (Assumed Stat) Must be float. How do we enforce?
        /// <summary>
        /// Suit Power (Stalker)
        /// </summary>
        Resource3           = 5,    // Must be float. How do we enforce?
        /// <summary>
        /// Spell Power (Spellslinger),
        /// </summary>
        Resource4           = 6,    // Must be float. How do we enforce?
        Resource5           = 7,    // (Assumed Stat) Must be float. How do we enforce?
        Resource6           = 8,    // (Assumed Stat) Must be float. How do we enforce?
        Dash                = 9,    // Must be float. How do we enforce?
        Level               = 10,
        MentorLevel         = 11,
        StandState          = 12, // 0 = Standing (Combat Pose), 1 = Sitting, 2 = Laying Down, 3 = Standing (Idle) More info: https://github.com/Hammster/wildstar-api-docs/blob/36be999b77a9dcdc4b27e95d217d54e419fdcbf3/Classes/Unit.md#getstandstate
        Sheathed            = 15, // 0 = Unsheathed Weapons, 1 = Sheathed Weapons
        Shield              = 20,
        // Stat(32) Duplicates Health, Stat(33) Duplicates Focus, and so on. Seems to loop at this point, suggesting a maximum of 32 "Stats".
    }
}
