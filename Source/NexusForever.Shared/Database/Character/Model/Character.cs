using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.Character.Model
{
    public partial class Character
    {
        public Character()
        {
            CharacterAppearance = new HashSet<CharacterAppearance>();
            CharacterCustomisation = new HashSet<CharacterCustomisation>();
        }

        public ulong Id { get; set; }
        public uint AccountId { get; set; }
        public string Name { get; set; }
        public byte Sex { get; set; }
        public byte Race { get; set; }
        public byte Class { get; set; }
        public byte Level { get; set; }
        public DateTime CreateTime { get; set; }

        public ICollection<CharacterAppearance> CharacterAppearance { get; set; }
        public ICollection<CharacterCustomisation> CharacterCustomisation { get; set; }
    }
}
