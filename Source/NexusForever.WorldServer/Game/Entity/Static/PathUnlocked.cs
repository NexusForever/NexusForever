using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Allows a flag to unlock <see cref="Path"/> for <see cref="Player"/>.
    /// </summary>
    [Flags]
    public enum PathUnlocked
    {
        Soldier     = 1,
        Settler     = 2,
        Scientist   = 4,
        Explorer    = 8
    }
}
