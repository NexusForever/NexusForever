using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class RealmConfigStartingLocation
    {
        public byte Id { get; set; }
        public byte Race { get; set; }
        public ushort FactionId { get; set; }
        public uint LocationId { get; set; }
        public byte CharacterCreationStart { get; set; }

        public virtual RealmConfig IdNavigation { get; set; }
    }
}
