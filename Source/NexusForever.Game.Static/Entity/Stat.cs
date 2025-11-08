namespace NexusForever.Game.Static.Entity
{
    public enum Stat : byte
    {
        [Stat(StatType.Integer, false)]
        Health              = 0,
        [Stat(StatType.Float)]
        Focus               = 1,
        /// <summary>
        /// Endurance
        /// </summary>
        [Stat(StatType.Float)]
        SprintEnergy        = 2,
        /// <summary>
        /// Kinetic Energy (Warrior), Psi Points (Esper), Volatile Energy (Engineer), Medic Cores (Medic)
        /// </summary>
        [Stat(StatType.Float)]
        Resource1           = 3,
        [Stat(StatType.Float)]
        Resource2           = 4,    // (Assumed Stat)
        /// <summary>
        /// Suit Power (Stalker)
        /// </summary>
        [Stat(StatType.Float)]
        Resource3           = 5,
        /// <summary>
        /// Spell Power (Spellslinger),
        /// </summary>
        [Stat(StatType.Float)]
        Resource4           = 6,
        [Stat(StatType.Float)]
        Resource5           = 7,    // (Assumed Stat)
        [Stat(StatType.Float)]
        Resource6           = 8,    // (Assumed Stat)
        [Stat(StatType.Float)]
        DashEnergy          = 9,
        [Stat(StatType.Integer)]
        Level               = 10,
        [Stat(StatType.Integer)]
        EffectiveLevel      = 11,
        [Stat(StatType.Integer, false)]
        StandState          = 12, // 0 = Standing (Combat Pose), 1 = Sitting, 2 = Laying Down, 3 = Standing (Idle) More info: https://github.com/Hammster/wildstar-api-docs/blob/36be999b77a9dcdc4b27e95d217d54e419fdcbf3/Classes/Unit.md#getstandstate
        [Stat(StatType.Integer, false)]
        Unknown13           = 13,
        [Stat(StatType.Integer, false)]
        Unknown14           = 14,
        [Stat(StatType.Integer)]
        WeaponSheatheState  = 15, // 0 = Unsheathed Weapons, 1 = Sheathed Weapons
        [Stat(StatType.Integer)]
        TargetCluster       = 17,
        [Stat(StatType.Float)]
        Breath              = 19,
        [Stat(StatType.Integer)]
        ShieldCapacity      = 20,
        [Stat(StatType.Integer)]
        InterruptArmour     = 21,
        [Stat(StatType.Integer)]
        Absorption          = 22,
        [Stat(StatType.Float)]
        Resource8           = 23,
        [Stat(StatType.Float)]
        Resource9           = 24,
        [Stat(StatType.Float)]
        Resource10          = 25,
        [Stat(StatType.Integer)]
        HealingAbsorption   = 26
    }
}
