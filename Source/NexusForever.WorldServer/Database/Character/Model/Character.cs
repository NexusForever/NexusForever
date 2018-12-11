using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class Character
    {
        public Character()
        {
            CharacterActionSetAmp = new HashSet<CharacterActionSetAmp>();
            CharacterActionSetShortcut = new HashSet<CharacterActionSetShortcut>();
            CharacterAppearance = new HashSet<CharacterAppearance>();
            CharacterBone = new HashSet<CharacterBone>();
            CharacterCostume = new HashSet<CharacterCostume>();
            CharacterCurrency = new HashSet<CharacterCurrency>();
            CharacterCustomisation = new HashSet<CharacterCustomisation>();
            CharacterDatacube = new HashSet<CharacterDatacube>();
            CharacterKeybinding = new HashSet<CharacterKeybinding>();
            CharacterMail = new HashSet<CharacterMail>();
            CharacterPath = new HashSet<CharacterPath>();
            CharacterPetCustomisation = new HashSet<CharacterPetCustomisation>();
            CharacterPetFlair = new HashSet<CharacterPetFlair>();
            CharacterQuest = new HashSet<CharacterQuest>();
            CharacterSpell = new HashSet<CharacterSpell>();
            CharacterStats = new HashSet<CharacterStats>();
            CharacterTitle = new HashSet<CharacterTitle>();
            CharacterZonemapHexgroup = new HashSet<CharacterZonemapHexgroup>();
            Item = new HashSet<Item>();
            CharacterReputation = new HashSet<CharacterReputation>();
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
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
        public ushort WorldId { get; set; }
        public ushort WorldZoneId { get; set; }
        public ushort Title { get; set; }
        public uint ActivePath { get; set; }
        public DateTime PathActivatedTimestamp { get; set; }
        public sbyte ActiveCostumeIndex { get; set; }
        public sbyte InputKeySet { get; set; }
        public byte ActiveSpec { get; set; }
        public uint TimePlayedTotal { get; set; }
        public uint TimePlayedLevel { get; set; }

        public virtual Residence Residence { get; set; }
        public virtual ICollection<CharacterActionSetAmp> CharacterActionSetAmp { get; set; }
        public virtual ICollection<CharacterActionSetShortcut> CharacterActionSetShortcut { get; set; }
        public virtual ICollection<CharacterAppearance> CharacterAppearance { get; set; }
        public virtual ICollection<CharacterBone> CharacterBone { get; set; }
        public virtual ICollection<CharacterCostume> CharacterCostume { get; set; }
        public virtual ICollection<CharacterCurrency> CharacterCurrency { get; set; }
        public virtual ICollection<CharacterCustomisation> CharacterCustomisation { get; set; }
        public virtual ICollection<CharacterDatacube> CharacterDatacube { get; set; }
        public virtual ICollection<CharacterKeybinding> CharacterKeybinding { get; set; }
        public virtual ICollection<CharacterMail> CharacterMail { get; set; }
        public virtual ICollection<CharacterPath> CharacterPath { get; set; }
        public virtual ICollection<CharacterPetCustomisation> CharacterPetCustomisation { get; set; }
        public virtual ICollection<CharacterPetFlair> CharacterPetFlair { get; set; }
        public virtual ICollection<CharacterQuest> CharacterQuest { get; set; }
        public virtual ICollection<CharacterSpell> CharacterSpell { get; set; }
        public virtual ICollection<CharacterStats> CharacterStats { get; set; }
        public virtual ICollection<CharacterTitle> CharacterTitle { get; set; }
        public virtual ICollection<CharacterZonemapHexgroup> CharacterZonemapHexgroup { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public ICollection<CharacterReputation> CharacterReputation { get; set; }
    }
}
