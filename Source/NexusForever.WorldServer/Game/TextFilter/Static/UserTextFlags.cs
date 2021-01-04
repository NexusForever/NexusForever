using System;

namespace NexusForever.WorldServer.Game.TextFilter.Static
{
    /// <remarks>
    /// This values come from the client user text parse function, see sub_72C0E0 for more information.
    /// </remarks>
    [Flags]
    public enum UserTextFlags
    {
        None               = 0x0000,
        NoConsecutiveSpace = 0x0001,
        NoStartEndSpace    = 0x0002,
        Unknown4           = 0x0004,
        Unknown8           = 0x0008,
        NoSpace            = 0x0010,
        Unknown20          = 0x0020,
        AllowMultiline     = 0x0040,
        Unknown80          = 0x0080,
        Unknown100         = 0x0100,
        AllowUnderscore    = 0x0200,
        RequireSpace       = 0x0400
    }
}
