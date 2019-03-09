using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPetFlair
    {
        public ulong Id { get; set; }
        public uint PetFlairId { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}
