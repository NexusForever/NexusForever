using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Character.Model
{
    public partial class CharacterCustomisation
    {
        public ulong Id { get; set; }
        public uint Label { get; set; }
        public uint Value { get; set; }

        public Character IdNavigation { get; set; }
    }
}
