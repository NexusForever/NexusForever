using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class Character
    {
        public Character()
        {
            CharacterAppearance = new HashSet<CharacterAppearance>();
            CharacterBone = new HashSet<CharacterBone>();
            CharacterCurrency = new HashSet<CharacterCurrency>();
            CharacterCustomisation = new HashSet<CharacterCustomisation>();
            Item = new HashSet<Item>();
        }

        public ulong Id { get; set; }
        public uint AccountId { get; set; }
        public string Name { get; set; }
        public byte Sex { get; set; }
        public byte Race { get; set; }
        public byte Class { get; set; }
        public byte Level { get; set; }
        public ushort FactionId { get; set; }
        public DateTime CreateTime { get; set; }

        public ICollection<CharacterAppearance> CharacterAppearance { get; set; }
        public ICollection<CharacterBone> CharacterBone { get; set; }
        public ICollection<CharacterCurrency> CharacterCurrency { get; set; }
        public ICollection<CharacterCustomisation> CharacterCustomisation { get; set; }
        public ICollection<Item> Item { get; set; }
    }
}
