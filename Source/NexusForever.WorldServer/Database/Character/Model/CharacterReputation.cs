using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterReputation
    {
        public ulong Id { get; set; }
        public uint FactionId { get; set; }
        public ulong Value { get; set; }

        public Character Character { get; set; }
    }
}
