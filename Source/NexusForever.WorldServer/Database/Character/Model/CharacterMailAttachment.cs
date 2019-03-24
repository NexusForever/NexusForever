using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterMailAttachment
    {
        public ulong Id { get; set; }
        public uint ItemId { get; set; }
        public uint Index { get; set; }
        public uint Amount { get; set; }

        public virtual CharacterMail IdNavigation { get; set; }
    }
}
