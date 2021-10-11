namespace NexusForever.WorldServer.Game.Spell.Static
{
    public enum DamageShape
    {
        Circle        = 0,
        Ring          = 1,
        Square        = 2,
        // 3 - Seems like a Cross shape
        Cone          = 4,
        /// <summary>
        /// This is a pie shape, where the radius value is the size of the missing slice.
        /// </summary>
        Pie           = 5,
        // 6 - Seems Aura related?
        Rectangle     = 7,
        LongCone      = 8
    }
}
