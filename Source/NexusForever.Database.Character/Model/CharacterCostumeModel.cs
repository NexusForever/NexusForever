using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class CharacterCostumeModel
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public uint Mask { get; set; }
        public DateTime Timestamp { get; set; }

        public CharacterModel Character { get; set; }
        public ICollection<CharacterCostumeItemModel> CostumeItem { get; set; } = new HashSet<CharacterCostumeItemModel>();
    }
}
