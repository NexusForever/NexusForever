using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterCurrency
    {
        public ulong Id { get; set; }
        public byte CurrencyId { get; set; }
        public ulong Count { get; set; }

        public Character IdNavigation { get; set; }
    }
}
