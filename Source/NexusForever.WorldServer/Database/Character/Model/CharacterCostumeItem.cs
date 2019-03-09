using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterCostumeItem
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public byte Slot { get; set; }
        public uint ItemId { get; set; }
        public int DyeData { get; set; }

        public virtual CharacterCostume I { get; set; }
    }
}
