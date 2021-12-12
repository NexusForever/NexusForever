namespace NexusForever.Game.Static.Spell
{
    /// <summary>
    /// This is used to indicate what flags a Target should have for a Base Spell to target it
    /// </summary>
    [Flags]
    public enum SpellTargetMechanicFlags
    {
        None               = 0x0000,
        IsPlayer           = 0x0001,
        IsFriendly         = 0x0008,
        IsEnemy            = 0x0010,
        AlsoIncludeEnemies = 0x0020, // Presumed that AoeCount is applied to Enemies and Friendlies individually
    }
}
