using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterTitle
    {
        public ulong Id { get; set; }
        public uint Title { get; set; }

        public Character Character { get; set; }
    }
}
