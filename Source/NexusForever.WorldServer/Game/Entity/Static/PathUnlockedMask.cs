using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Allows a flag to unlock <see cref="Path"/> for <see cref="Player"/>.
    /// </summary>
    [Flags]
    public enum PathUnlockedMask
    {
        None      = 0,
        Soldier   = 1 << Path.Soldier,
        Settler   = 1 << Path.Settler,
        Scientist = 1 << Path.Scientist,
        Explorer  = 1 << Path.Explorer
    }
}
