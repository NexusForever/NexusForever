using System;

namespace NexusForever.WorldServer.Game.Mail.Static
{
    /// <summary>
    /// Used to set certain states on the <see cref="MailItem"/>
    /// </summary>
    [Flags]
    public enum MailFlag
    {
        None          = 0x00,
        IsRead        = 0x02,
        IsSaved       = 0x04,
        NotReturnable = 0x08,
        NoExpiry      = 0x20,
        IsGift        = 0x40
    }
}
