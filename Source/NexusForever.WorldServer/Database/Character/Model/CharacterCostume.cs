using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterCostume
    {
        public CharacterCostume()
        {
            CharacterCostumeItem = new HashSet<CharacterCostumeItem>();
        }

        public ulong Id { get; set; }
        public byte Index { get; set; }
        public uint Mask { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Character IdNavigation { get; set; }
        public virtual ICollection<CharacterCostumeItem> CharacterCostumeItem { get; set; }
    }
}
