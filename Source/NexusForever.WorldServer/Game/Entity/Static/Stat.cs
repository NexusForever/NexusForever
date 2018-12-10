namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum Stat
    {
        Health              = 0,
        Focus               = 1,    // Must be float. How do we enforce?
        Endurance           = 2,    // Must be float. How do we enforce?
        /// <summary>
        /// Kinetic Energy (Warrior), Psi Points (Esper), Volatile Energy (Engineer), Medic Cores (Medic)
        /// </summary>
        Resource1           = 3,    // Must be float. How do we enforce?
        /// <summary>
        /// Suit Power (Stalker)
        /// </summary>
        Resource3           = 5,    // Must be float. How do we enforce?
        /// <summary>
        /// Spell Power (Spellslinger),
        /// </summary>
        Resource4           = 6,    // Must be float. How do we enforce?
        Dash                = 9,    // Must be float. How do we enforce?
        Level               = 10,
        MentorLevel         = 11,
        State               = 12, // 0 = Standing (Combat Pose), 1 = Sitting, 2 = Laying Down, 3 = Standing (Idle)
        Sheathed            = 15, // 0 = Unsheathed Weapons, 1 = Sheathed Weapons
        Shield              = 20,
        LowHpAndGhostEffect = 32, // Need confirmation what screen effects are happening
    }
}
