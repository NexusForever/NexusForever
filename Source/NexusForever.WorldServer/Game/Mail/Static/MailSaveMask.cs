using System;

namespace NexusForever.WorldServer.Game.Mail.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="MailItem"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum MailSaveMask
    {
        None                = 0x0000,
        Create              = 0x0001,
        Flags               = 0x0002,
        CurrencyChange      = 0x0004,
        RecipientChange     = 0x0008,
        Delete              = 0x0010
    }
}
