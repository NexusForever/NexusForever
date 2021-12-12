namespace NexusForever.Game.Static.Spell
{
    /// <summary>
    /// This appears to be an indicator of the location at which target acquisition begins.
    /// </summary>
    /// <remarks>Unsure exactly how this works at this time. It appears to be an optimisation to allow for quicker, accurate selection. Needs investigation.</remarks>
    public enum SpellTargetMechanicType
    {
        Self            = 0,
        PrimaryTarget   = 1,
        SecondaryTarget = 2
    }
}
