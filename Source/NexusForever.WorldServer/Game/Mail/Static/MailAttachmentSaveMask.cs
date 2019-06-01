using System;

namespace NexusForever.WorldServer.Game.Mail.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="MailAttachment"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum MailAttachmentSaveMask
    {
        None   = 0x0000,
        Create = 0x0001,
        Delete = 0x0002
    }
}
