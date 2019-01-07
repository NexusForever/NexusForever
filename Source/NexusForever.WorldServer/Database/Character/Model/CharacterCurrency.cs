using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterCurrency
    {
        public ulong Id { get; set; }
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }

        public Character IdNavigation { get; set; }
    }
}
