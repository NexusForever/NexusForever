using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPetCustomisation
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public uint ObjectId { get; set; }
        public string Name { get; set; }
        public ulong FlairIdMask { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}
