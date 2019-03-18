using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Mail.Static
{
    [Flags]
    public enum MailFlag
    {
        None            = 0x00,
        IsRead          = 0x02,
        IsSaved         = 0x04,
        IsReturnable    = 0x08,
        NoExpiry        = 0x20,
        IsGift          = 0x40,
    }
}
